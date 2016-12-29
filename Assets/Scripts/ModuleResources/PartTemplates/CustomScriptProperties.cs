﻿using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.ModuleResources.PartTemplates
{
    public class CustomScriptProperties : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

        public void AddProperty(string name, object value)
        {
            properties.Add(name, value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}