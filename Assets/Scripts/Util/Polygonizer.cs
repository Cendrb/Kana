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
            points.Add(vertices[edge.Vertex1]);

            // Remove this edge
            edges.Remove(edge);

            // Find next edge that contains vert2
            foreach (ModelPartEdge nextEdge in edges)
            {
                if (nextEdge.Vertex1 == edge.Vertex2)
                {
                    edgeTrace(nextEdge);
                    return;
                }
            }

            // No next edge found, create a path based on these points
            paths.Add(currentPathIndex, points.ToArray());

            // Empty path
            points.Clear();

            // Increment path index
            currentPathIndex++;

            // Start next edge trace if there are edges left
            if (edges.Count > 0)
            {
                edgeTrace(edges[0]);
            }
        }

        public Dictionary<int, Vector2[]> GetPaths()
        {
            return paths;
        }
    }
}
