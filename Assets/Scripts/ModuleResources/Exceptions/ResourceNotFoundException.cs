﻿using System;

namespace Assets.Scripts.ModuleResources.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceLocation Resource { get; private set; }

        public ResourceNotFoundException(ResourceLocation resource)
        {
            this.Resource = resource;
        }

        public override string ToString()
        {
            return string.Format("Resource {0} of type {1} was not found",
                this.Resource.ToResourceLocationString(),
                this.Resource.Type);
        }
    }
}
