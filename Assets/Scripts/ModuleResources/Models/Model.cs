using System.Collections.Generic;

namespace Assets.Scripts.ModuleResources.Models
{
    public class Model
    {
        public ResourceLocation ResourceLocation { get; set; }
        public ResourceLocation Texture { get; set; }
        public int RenderLayer { get; set; }
        public List<ModelPart> Parts { get; private set; }

        public Model(ResourceLocation resourceLocation, ResourceLocation texture, int renderLayer, List<ModelPart> parts)
        {
            ResourceLocation = resourceLocation;
            Texture = texture;
            RenderLayer = renderLayer;
            Parts = parts;
        }
    }
}
