using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.GameResources;
using Assets.Scripts.ModuleResources;
using Assets.Scripts.ModuleResources.Models;
using Assets.Scripts.ModuleResources.PartTemplates;
using Assets.Scripts.Util;
using UnityEngine;
using Joint = Assets.Scripts.GameResources.Joint;
using ResourceLocation = Assets.Scripts.ModuleResources.ResourceLocation;

namespace Assets.Scripts.PartScripts
{
    public class Part : MonoBehaviour
    {
        public string LocalizedName { get; private set; }
        public ResourceLocation ResourceLocation { get; private set; }
        public ShopProperties ShopProperties { get; private set; }
        public ScriptProperties ScriptProperties { get; private set; }
        protected List<RenderedModel> Models { get; private set; }
        protected Vehicle ParentVehicle;
        public List<Joint> JointPoints = new List<Joint>();

        public Part()
        {
        }

        public void LoadFrom(PartTemplate template, Vehicle parentVehicle)
        {
            ParentVehicle = parentVehicle;
            ResourceLocation = template.ResourceLocation;
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

            List<RenderedModel> renderedModels = Models.Where(model => model.RenderOnDefault).ToList();

            foreach (RenderedModel model in renderedModels)
            {
                initGOForModel(model, gameObject);
            }
        }

        private void initGOForModel(RenderedModel model, GameObject parentGameObject)
        {
            GameObject modelGameObject = new GameObject(model.ResourceLocation.ToResourceLocationString());
            modelGameObject.transform.SetParent(parentGameObject.transform, false);
            modelGameObject.transform.localPosition = model.Relative;

            // MESH
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
                int[] tris = part.GetTrisForMesh();
                int[] globalizedTris = tris.Select(triInt => triInt + vertices.Count).ToArray();
                Vector3[] verts = part.GetVerticesForMesh();
                if (part.Collide)
                {
                    collisionTris.AddRange(globalizedTris);
                    collisionVerts.AddRange(verts);
                }
                submeshesTris.Add(partIndex, globalizedTris);
                vertices.AddRange(verts);
                uvs.AddRange(part.GetUVsForMesh());
                partIndex++;


                // EDGES AND JOINTS
                List<ModelPartEdge> edges = new List<ModelPartEdge>();
                for (int i = 0; i < verts.Length; i++)
                {
                    if (i + 1 < verts.Length)
                        edges.Add(new ModelPartEdge(i, i + 1));
                    else
                        edges.Add(new ModelPartEdge(i, 0));
                }
                int[] joints = part.GetJointsForMesh();
                for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
                {
                    ModelPartEdge edge = edges[edgeIndex];
                    int jointsPerEdge = joints[edgeIndex];
                    Vector2 coor1 = verts[edge.Vertex1];
                    Vector2 coor2 = verts[edge.Vertex2];
                    Vector2 edgeVector = coor2 - coor1;
                    Vector2 partEdgeVector = edgeVector / (jointsPerEdge + 1);
                    float jointRotation = VectorUtil.NormalizeAngle(VectorUtil.AngleBetweenVector2(new Vector2(0, 1), edgeVector) - 90);
                    for (int i = 0; i < jointsPerEdge; i++)
                    {
                        Vector2 jointPosition = (coor1 + (i + 1) * partEdgeVector) + part.Relative + model.Relative;
                        JointPoints.Add(new Joint(jointPosition, jointRotation));
                    }
                }
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


            // TEXTURES
            Material material = ModuleLoader.GetMaterial(model.Texture);
            MeshRenderer meshRenderer = modelGameObject.AddComponent<MeshRenderer>();
            meshRenderer.materials =
                Enumerable.Repeat<Material>(material, parts.Count).ToArray();


            // POLYGON COLLIDER
            PolygonCollider2D polygonCollider = modelGameObject.AddComponent<PolygonCollider2D>();
            List<ModelPartEdge> colliderEdges = new List<ModelPartEdge>();
            for (int i = 0; i < collisionTris.Count; i += 3)
            {
                Extensions.AddIfNotExists(colliderEdges, new ModelPartEdge(collisionTris[i], collisionTris[i + 1]));
                Extensions.AddIfNotExists(colliderEdges, new ModelPartEdge(collisionTris[i + 1], collisionTris[i + 2]));
                Extensions.AddIfNotExists(colliderEdges, new ModelPartEdge(collisionTris[i + 2], collisionTris[i]));
            }

            Polygonizer polygonizer = new Polygonizer(colliderEdges, collisionVerts.Select(v3 => (Vector2)v3).ToArray());
            Dictionary<int, Vector2[]> paths = polygonizer.GetPaths();
            polygonCollider.pathCount = paths.Count;
            foreach (KeyValuePair<int, Vector2[]> pair in paths)
            {
                polygonCollider.SetPath(pair.Key, pair.Value);
            }
        }
    }
}
