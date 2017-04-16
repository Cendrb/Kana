using UnityEngine;

namespace Assets.Scripts.ModuleResources.Models
{
    public class ModelPart
    {
        public Vector2 Relative { get; private set; }
        public bool Collide { get; private set; }
        private readonly Vector2[] vertices;
        private readonly Vector2[] uvs;
        private readonly int[] joints;
        private readonly int[] triangles;

        public ModelPart(Vector2 relative, bool collide, Vector2[] vertices, Vector2[] uvs, int[] joints)
        {
            this.Relative = relative;
            this.Collide = collide;
            this.vertices = vertices;
            this.uvs = uvs;
            this.joints = joints;

            Triangulator triangulator = new Triangulator(this.vertices);
            this.triangles = triangulator.Triangulate();
        }

        public Vector3[] GetVerticesForMesh()
        {
            Vector3[] v3Vertices = new Vector3[this.vertices.Length];
            int index = 0;
            foreach (Vector2 vector2 in this.vertices)
            {
                v3Vertices[index] = vector2 + this.Relative;
                index++;
            }
            return v3Vertices;
        }

        public Vector2[] GetUVsForMesh()
        {
            return this.uvs;
        }

        public int[] GetTrisForMesh()
        {
            return this.triangles;
        }

        public int[] GetJointsForMesh()
        {
            return this.joints;
        }
    }
}
