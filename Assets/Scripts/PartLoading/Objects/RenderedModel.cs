using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.PartLoading.Objects
{
    public class RenderedModel : Model
    {
        public Vector2 Relative { get; set; }

        public RenderedModel(string module, string name, string texture, bool renderOnDefault, List<ModelPart> parts, Vector2 relative) : base(module, name, texture, renderOnDefault, parts)
        {
            Relative = relative;
        }

        public static RenderedModel CreateFromSubModel(Model subModel)
        {
            return new RenderedModel(subModel.Module, subModel.Name, subModel.Texture, subModel.RenderOnDefault,subModel.Parts, new Vector2(0, 0));
        }
    }
}
