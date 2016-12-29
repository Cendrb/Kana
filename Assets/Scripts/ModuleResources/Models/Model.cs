﻿using System.Collections.Generic;

namespace Assets.Scripts.ModuleResources.Models
{
    public class Model
    {
        public string Module { get; private set; }
        public string Name { get; private set; } // required in submodel (property in root)
        public ResourceLocation Texture { get; set; } // required in submodel (property in root)
        public bool RenderOnDefault { get; set; } // required in submodel (property in root)
        public List<ModelPart> Parts { get; private set; }

        public Model(string module, string name, ResourceLocation texture, bool renderOnDefault, List<ModelPart> parts)
        {
            Module = module;
            Name = name;
            Texture = texture;
            RenderOnDefault = renderOnDefault;
            Parts = parts;
        }
    }
}