using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.PartLoading;
using Assets.Scripts.PartLoading.Objects;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.PartScripts
{
    public class Part : MonoBehaviour
    {
        public string UnlocalizedName { get; private set; }
        public string LocalizedName { get; private set; }
        public ShopProperties ShopProperties { get; private set; }
        public ScriptProperties ScriptProperties { get; private set; }
        protected List<RenderedModel> Models { get; private set; }

        public Part()
        {
        }

        public void LoadFrom(PartTemplate template)
        {
            UnlocalizedName = template.UnlocalizedName;
            LocalizedName = template.LocalizedName;
            ShopProperties = template.ShopProp; // TODO create new instance instead of linking?
            ScriptProperties = template.ScriptProp; // TODO create new instance instead of linking?
            Models = template.Models;
            Type thisType = GetType();
            foreach (KeyValuePair<string, object> customScriptProperty in template.CustomScriptProp)
            {
                PropertyInfo propertyInfo = thisType.GetProperty(customScriptProperty.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(this, customScriptProperty.Value, null);
                }
                else
                    Log.Error("Part#LoadFrom", string.Format("Property {0} was not found on type {1}", customScriptProperty.Key, thisType.Name));
            }
        }

        void Start()
        {
            GameObject meshGameObject = new GameObject("Meshes");
            meshGameObject.transform.SetParent(gameObject.transform, false);
            GameObject colliderGameObject = new GameObject("Collider");
            colliderGameObject.transform.SetParent(meshGameObject.transform, false);
            GameObject noncolliderGameObject = new GameObject("Noncollider");
            noncolliderGameObject.transform.SetParent(meshGameObject.transform, false);

            Material material = Resources.Load<Material>("Materials/default");

            List<RenderedModel> renderedModels = Models.Where(model => model.RenderOnDefault).ToList();

            List<ModelPart> colliderParts = new List<ModelPart>();
            List<ModelPart> noncolliderParts = new List<ModelPart>();
            foreach (RenderedModel model in renderedModels)
            {
                foreach (ModelPart modelPart in model.Parts)
                {
                    if (modelPart.Collide)
                        colliderParts.Add(modelPart);
                    else
                        noncolliderParts.Add(modelPart);
                }
            }

            initGOForParts(colliderParts, colliderGameObject, true);
            initGOForParts(noncolliderParts, noncolliderGameObject, false);
        }

        private void initGOForParts(List<ModelPart> parts, GameObject colliderGameObject, bool collide)
        {
            Mesh mesh = new Mesh();
            mesh.subMeshCount = parts.Count;
            Dictionary<int, int[]> submeshesTris = new Dictionary<int, int[]>();
            List<Vector3> vertices = new List<Vector3>();
            int partIndex = 0;
            foreach (ModelPart part in parts)
            {
                submeshesTris.Add(partIndex, part.GetTrisForMesh().Select(triInt => triInt + vertices.Count).ToArray());
                vertices.AddRange(part.GetVerticesForMesh());
                partIndex++;
            }
            mesh.vertices = vertices.ToArray();

            foreach (KeyValuePair<int, int[]> submeshTri in submeshesTris)
            {
                mesh.SetTriangles(submeshTri.Value, submeshTri.Key);
            }

            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            colliderGameObject.AddComponent<MeshFilter>().mesh = mesh;

            MeshRenderer meshRenderer = colliderGameObject.AddComponent<MeshRenderer>();
            // TODO load real texture
            meshRenderer.materials =
                Enumerable.Repeat<Material>(Resources.Load<Material>("Materials/default"), parts.Count).ToArray();

            if (collide)
            {
                PolygonCollider2D polygonCollider = colliderGameObject.AddComponent<PolygonCollider2D>();

                // EDGES
                List<ModelPartEdge> edges = new List<ModelPartEdge>();
                int[] triangles = mesh.triangles;
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    Extensions.AddIfNotExists(edges, new ModelPartEdge(triangles[i], triangles[i + 1]));
                    Extensions.AddIfNotExists(edges, new ModelPartEdge(triangles[i + 1], triangles[i + 2]));
                    Extensions.AddIfNotExists(edges, new ModelPartEdge(triangles[i + 2], triangles[i]));
                }

                Polygonizer polygonizer = new Polygonizer(edges, vertices.Select(v3 => (Vector2)v3).ToArray());
                Dictionary<int, Vector2[]> paths = polygonizer.GetPaths();
                polygonCollider.pathCount = paths.Count;
                foreach (KeyValuePair<int, Vector2[]> pair in paths)
                {
                    polygonCollider.SetPath(pair.Key, pair.Value);
                }
            }
        }
    }
}
