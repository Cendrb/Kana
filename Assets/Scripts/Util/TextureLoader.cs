using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Util
{
    static class TextureLoader
    {
        private static Material defaultMaterial;

        static TextureLoader()
        {
            defaultMaterial = Resources.Load<Material>("Materials/default");
        }
    }
}
