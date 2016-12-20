using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.PartLoading.Objects
{
    public class ScriptProperties
    {
        public float Mass { get; private set; }
        public int Health { get; private set; }
        public int DamageOnTouch { get; private set; }

        public ScriptProperties(float mass, int health, int damageOnTouch)
        {
            Mass = mass;
            Health = health;
            DamageOnTouch = damageOnTouch;
        }
    }
}
