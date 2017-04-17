using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.GameResources
{
    public class CenterOfMass
    {
        public float Mass { get; set; }
        public Vector2 CenterOf { get; set; }

        public CenterOfMass(Vector2 centerOf, float mass)
        {
            this.Mass = mass;
            this.CenterOf = centerOf;
        }
    }
}
