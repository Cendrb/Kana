using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.ModuleResources.Exceptions
{
    class PropertyReadException : Exception
    {
        public string PropertyName { get; set; }
        public JObject ParentObject { get; set; }
        public string FoundValue { get; set; }
        public Type RequiredType { get; set; }

        public PropertyReadException(string propertyName, JObject parentObject, string foundValue, Type requiredType)
        {
            this.PropertyName = propertyName;
            this.ParentObject = parentObject;
            this.FoundValue = foundValue;
            this.RequiredType = requiredType;
        }

        public override string ToString()
        {
            if (this.FoundValue == null)
            {
                return string.Format("Property missing. {0} is a required property of type {1}. Searched in: root.{2}", this.PropertyName, TypeToReadeable(this.RequiredType), this.ParentObject.Path);
            }
            else
            {
                return string.Format("Property of an invalid type. {0} needs to be {1} ({3} found). Searched in: {2}", this.PropertyName, TypeToReadeable(this.RequiredType), this.ParentObject.Path, this.FoundValue);
            }
        }

        private string TypeToReadeable(Type type)
        {
            if (type == typeof(JObject))
                return "JSON Object";
            else if (type == typeof(JArray))
                return "JSON Array";
            else if (type == typeof(int))
                return "integer";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(string))
                return "string";
            else if (type == typeof(int[]))
                return "Array of ints";
            else if (type == typeof(float[]))
                return "Array of floats";
            else if (type == typeof(Vector2))
                return "Array of two floats";
            else
                return type.Name;
        }       
    }
}
