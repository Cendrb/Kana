﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.PartLoading
{
    class InvalidPropertyNameException : Exception
    {
        public string PropertyName { get; private set; }
        public JObject ParentJObject { get; private set; }

        public InvalidPropertyNameException(string propertyName, JObject parentJObject)
        {
            PropertyName = propertyName;
            ParentJObject = parentJObject;
        }

        public override string ToString()
        {
            return string.Format("JSON porperty name cannot be {0}. Parent JObject: {1}", PropertyName, ParentJObject.Path);
        }
    }
}
