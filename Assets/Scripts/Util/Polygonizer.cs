using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ModuleResources.Models;
using UnityEngine;

namespace Assets.Scripts.Util
{
   public class Polygonizer
    {
        private readonly List<ModelPartEdge> edges;
        private readonly Vector2[] vertices;
        private readonly Dictionary<int, Vector2[]> paths = new Dictionary<int, Vector2[]>();
        private readonly List<Vector2> points = new List<Vector2>();
        private int currentPathIndex = 0;

        public Polygonizer(List<ModelPartEdge> edges, Vector2[] vertices)
        {
            this.edges = edges;
            this.vertices = vertices;

            edgeTrace(edges[0]);
        }

        void edgeTrace(ModelPartEdge edge)
        {
            // Add this edge's vert1 coords to the point list
            this.points.Add(this.vertices[edge.Vertex1]);

            // Remove this edge
            this.edges.Remove(edge);

            // Find next edge that contains vert2
            foreach (ModelPartEdge nextEdge in this.edges)
            {
                if (nextEdge.Vertex1 == edge.Vertex2)
                {
                    edgeTrace(nextEdge);
                    return;
                }
            }

            // No next edge found, create a path based on these points
            this.paths.Add(this.currentPathIndex, this.points.ToArray());

            // Empty path
            this.points.Clear();

            // Increment path index
            this.currentPathIndex++;

            // Start next edge trace if there are edges left
            if (this.edges.Count > 0)
            {
                edgeTrace(this.edges[0]);
            }
        }

        public Dictionary<int, Vector2[]> GetPaths()
        {
            return this.paths;
        }
    }
}
