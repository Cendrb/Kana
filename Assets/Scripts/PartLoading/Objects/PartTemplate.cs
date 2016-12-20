using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.PartLoading.Objects;

namespace Assets.Scripts.PartLoading
{
    class PartTemplate
    {
        public string ModuleName { get; private set; }
        public string ScriptName { get; private set; }
        public string UnlocalizedName { get; private set; }
        public string LocalizedName { get; private set; }

        public ShopProperties ShopProp { get; private set; }
        public ScriptProperties ScriptProp { get; private set; }

        public List<RenderedModel> Models { get; private set; }

        public PartTemplate(string moduleName, string scriptName, string unlocalizedName, string localizedName, ShopProperties shopProperties, ScriptProperties scriptProperties, List<RenderedModel> models)
        {
            ModuleName = moduleName;
            ScriptName = scriptName;
            UnlocalizedName = unlocalizedName;
            LocalizedName = localizedName;
            ShopProp = shopProperties;
            ScriptProp = scriptProperties;
            Models = models;
        }

        public class ShopProperties
        {
            public int Cost { get; private set; }
            public int RequiredLevel { get; private set; }

            public ShopProperties(int cost, int requiredLevel)
            {
                Cost = cost;
                RequiredLevel = requiredLevel;
            }
        }

        public class ScriptProperties
        {
            private Dictionary<string, object> properties = new Dictionary<string, object>();

            public void AddProperty(string name, object value)
            {
                properties.Add(name, value);
            }
        }
    }
}
