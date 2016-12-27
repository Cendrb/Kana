using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.PartLoading.Objects
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
            Relative = relative;
            Collide = collide;
            this.vertices = vertices;
            this.uvs = uvs;
            this.joints = joints;

            Triangulator triangulator = new Triangulator(this.vertices);
            triangles = triangulator.Triangulate();
        }

        public Vector3[] GetVerticesForMesh()
        {
            Vector3[] v3Vertices = new Vector3[vertices.Length];
            int index = 0;
            foreach (Vector2 vector2 in vertices)
            {
                v3Vertices[index] = vector2 + Relative;
                index++;
            }
            return v3Vertices;
        }

        public Vector2[] GetUVsForMesh()
        {
            return uvs;
        }

        public int[] GetTrisForMesh()
        {
            return triangles;
        }
    }
}
