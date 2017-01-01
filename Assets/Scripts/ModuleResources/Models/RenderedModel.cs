using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ModuleResources.Models
{
    public class RenderedModel : Model
    {
        public Vector2 Relative { get; set; }

        public RenderedModel(ResourceLocation resourceLocation, ResourceLocation texture, bool renderOnDefault, List<ModelPart> parts, Vector2 relative) : base(resourceLocation, texture, renderOnDefault, parts)
        {
            Relative = relative;
        }

        public static RenderedModel CreateFromSubModel(Model subModel)
        {
            return new RenderedModel(subModel.ResourceLocation, subModel.Texture, subModel.RenderOnDefault,subModel.Parts, new Vector2(0, 0));
        }
    }
}
