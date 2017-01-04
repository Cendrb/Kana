using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.GameResources
{
    public class Joint
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public Joint()
        {
        }

        public Joint(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
