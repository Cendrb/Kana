using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.PartScripts
{
    class Cannon : Part
    {
        public int ShootingSpeed { get; private set; }
        public int BulletSpeed { get; private set; }

        public Cannon()
        {
        }
    }
}
