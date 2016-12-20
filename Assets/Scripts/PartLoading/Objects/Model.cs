﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.PartLoading.Objects;

namespace Assets.Scripts.PartLoading
{
    class Model
    {
        public string Module { get; private set; }
        public string Name { get; private set; } // required in submodel (name of the file)
        public string Texture { get; set; } // required in submodel (property in root)
        public bool RenderOnDefault { get; set; } // required in submodel (property in root)
        public List<ModelPart> Parts { get; private set; }

        public Model(string module, string name, string texture, bool renderOnDefault, List<ModelPart> parts)
        {
            Module = module;
            Name = name;
            Texture = texture;
            RenderOnDefault = renderOnDefault;
            Parts = parts;
        }
    }
}