using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Util.Resources
{
    public class MaterialLoader : ResourceLoader<Material>
    {
        private static readonly Material DEFAULT_MATERIAL = UnityEngine.Resources.Load<Material>("Materials/default");

        protected override Material loadResource(ResourceLocation resourceLocation)
        {
            string resourcePath = resourceLocation.GetPath();
            byte[] bytes = File.ReadAllBytes(resourcePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            
            Material material = new Material(DEFAULT_MATERIAL);
            material.mainTexture = texture;
            return material;
        }
    }
}
