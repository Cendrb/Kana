﻿using System;

namespace Assets.Scripts.ModuleResources.Exceptions
{
    public class ResourceLocationParseException : Exception
    {
        public string ResourceLocationString { get; private set; }

        public ResourceLocationParseException(string resourceLocationString)
        {
            this.ResourceLocationString = resourceLocationString;
        }

        public override string ToString()
        {
            return string.Format("Resource location {0} is not valid. (module:name syntax is required)", this.ResourceLocationString);
        }
    }
}