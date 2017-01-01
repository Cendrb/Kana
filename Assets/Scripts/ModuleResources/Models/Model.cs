using System.Collections.Generic;

namespace Assets.Scripts.ModuleResources.Models
{
    public class Model
    {
        public ResourceLocation ResourceLocation { get; set; }
        public ResourceLocation Texture { get; set; }
        public bool RenderOnDefault { get; set; }
        public List<ModelPart> Parts { get; private set; }

        public Model(ResourceLocation resourceLocation, ResourceLocation texture, bool renderOnDefault, List<ModelPart> parts)
        {
            ResourceLocation = resourceLocation;
            Texture = texture;
            RenderOnDefault = renderOnDefault;
            Parts = parts;
        }
    }
}
