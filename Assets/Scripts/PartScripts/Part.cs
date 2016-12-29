using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.ModuleResources;
using Assets.Scripts.ModuleResources.Models;
using Assets.Scripts.ModuleResources.PartTemplates;
using Assets.Scripts.Util;
using UnityEngine;
using ResourceLocation = Assets.Scripts.ModuleResources.ResourceLocation;

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
            GameObject meshGameObject = new GameObject("Models");
            meshGameObject.transform.SetParent(gameObject.transform, false);

            List<RenderedModel> renderedModels = Models.Where(model => model.RenderOnDefault).ToList();

            foreach (RenderedModel model in renderedModels)
            {
               initGOForModel(model, meshGameObject);
            }
        }

        private void initGOForModel(Model model, GameObject parentGameObject)
        {
            GameObject modelGameObject = new GameObject(model.Name);
            modelGameObject.transform.SetParent(parentGameObject.transform, false);

            List<ModelPart> parts = model.Parts;
            Mesh mesh = new Mesh();
            mesh.subMeshCount = parts.Count;
            Dictionary<int, int[]> submeshesTris = new Dictionary<int, int[]>();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            List<int> collisionTris = new List<int>();
            List<Vector3> collisionVerts = new List<Vector3>();

            int partIndex = 0;
            foreach (ModelPart part in parts)
            {
                int[] tris = part.GetTrisForMesh().Select(triInt => triInt + vertices.Count).ToArray();
                Vector3[] verts = part.GetVerticesForMesh();
                if (part.Collide)
                {
                    collisionTris.AddRange(tris);
                    collisionVerts.AddRange(verts);
                }
                submeshesTris.Add(partIndex, tris);
                vertices.AddRange(verts);
                uvs.AddRange(part.GetUVsForMesh());
                partIndex++;
            }
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();

            foreach (KeyValuePair<int, int[]> submeshTri in submeshesTris)
            {
                mesh.SetTriangles(submeshTri.Value, submeshTri.Key);
            }

            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            modelGameObject.AddComponent<MeshFilter>().mesh = mesh;

            Material material = ModuleLoader.GetMaterial(model.Texture);
            MeshRenderer meshRenderer = modelGameObject.AddComponent<MeshRenderer>();
            meshRenderer.materials =
                Enumerable.Repeat<Material>(material, parts.Count).ToArray();


            // POLYGON COLLIDER
            PolygonCollider2D polygonCollider = modelGameObject.AddComponent<PolygonCollider2D>();
            List<ModelPartEdge> edges = new List<ModelPartEdge>();
            for (int i = 0; i < collisionTris.Count; i += 3)
            {
                Extensions.AddIfNotExists(edges, new ModelPartEdge(collisionTris[i], collisionTris[i + 1]));
                Extensions.AddIfNotExists(edges, new ModelPartEdge(collisionTris[i + 1], collisionTris[i + 2]));
                Extensions.AddIfNotExists(edges, new ModelPartEdge(collisionTris[i + 2], collisionTris[i]));
            }

            Polygonizer polygonizer = new Polygonizer(edges, collisionVerts.Select(v3 => (Vector2)v3).ToArray());
            Dictionary<int, Vector2[]> paths = polygonizer.GetPaths();
            polygonCollider.pathCount = paths.Count;
            foreach (KeyValuePair<int, Vector2[]> pair in paths)
            {
                polygonCollider.SetPath(pair.Key, pair.Value);
            }
        }
    }
}
