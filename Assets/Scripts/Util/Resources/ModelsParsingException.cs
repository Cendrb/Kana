﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Util.Resources
{
    public class ModelsParsingException : Exception
    {
        public ResourceLocation ParentResourceLocation { get; private set; }
        public int ModelIndex { get; private set; }

        public ModelsParsingException(ResourceLocation parentResourceLocation, int modelIndex)
        {
            ParentResourceLocation = parentResourceLocation;
            ModelIndex = modelIndex;
        }

        public override string ToString()
        {
            return string.Format("Failed to load model from part template {0} with the index of {1}", ParentResourceLocation.ToResourceLocationString(), ModelIndex);
        }
    }
}
