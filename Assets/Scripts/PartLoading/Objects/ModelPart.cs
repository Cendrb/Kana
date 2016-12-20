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
        private Vector2[] Vertices { get; set; }
        private int[] Joints { get; set; }

        public ModelPart(Vector2 relative, bool collide, Vector2[] vertices, int[] joints)
        {
            Relative = relative;
            Collide = collide;
            Vertices = vertices;
            Joints = joints;
        }
    }
}
