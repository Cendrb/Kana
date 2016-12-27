using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.PartLoading.Objects
{
    public class ModelPartEdge : IEquatable<ModelPartEdge>
    {
        public int Vertex1 { get; private set; }
        public int Vertex2 { get; private set; }

        public ModelPartEdge(int vertex1, int vertex2)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }

        public bool Equals(ModelPartEdge other)
        {
            if (other == null)
                return false;
            return this.Vertex1 == other.Vertex1 && this.Vertex2 == other.Vertex2;
        }
    }
}
