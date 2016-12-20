using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.PartLoading.Objects
{
    public class CustomScriptProperties
    {
        private Dictionary<string, object> properties = new Dictionary<string, object>();

        public void AddProperty(string name, object value)
        {
            properties.Add(name, value);
        }
    }
}
