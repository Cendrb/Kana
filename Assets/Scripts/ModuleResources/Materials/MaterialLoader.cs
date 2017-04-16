using System.IO;
using UnityEngine;

namespace Assets.Scripts.ModuleResources.Materials
{
    public class MaterialLoader : ResourceLoader<Material>
    {
        private static readonly Material DEFAULT_MATERIAL = UnityEngine.Resources.Load<Material>("Materials/default");

        protected override Material LoadResource(ResourceLocation resourceLocation)
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
