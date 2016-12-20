using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.PartLoading.Objects;
using Assets.Scripts.PartScripts;

namespace Assets.Scripts.PartLoading
{
    public class PartTemplate
    {
        public string ModuleName { get; private set; }
        public Type ScriptType { get; private set; }
        public string UnlocalizedName { get; private set; }
        public string LocalizedName { get; private set; }

        public ShopProperties ShopProp { get; private set; }
        public ScriptProperties ScriptProp { get; private set; }
        public CustomScriptProperties CustomScriptProp { get; private set; }

        public List<RenderedModel> Models { get; private set; }

        public PartTemplate(string moduleName, Type scriptType, string unlocalizedName, string localizedName, ShopProperties shopProperties, ScriptProperties scriptProperties, CustomScriptProperties customScriptProperties, List<RenderedModel> models)
        {
            ModuleName = moduleName;
            ScriptType = scriptType;
            UnlocalizedName = unlocalizedName;
            LocalizedName = localizedName;
            ShopProp = shopProperties;
            ScriptProp = scriptProperties;
            CustomScriptProp = customScriptProperties;
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
            public float Mass { get; private set; }
            public int Health { get; private set; }
            public int DamageOnTouch { get; private set; }

            public ScriptProperties(float mass, int health, int damageOnTouch)
            {
                Mass = mass;
                Health = health;
                DamageOnTouch = damageOnTouch;
            }
        }

        public class CustomScriptProperties
        {
            private Dictionary<string, object> properties = new Dictionary<string, object>();

            public void AddProperty(string name, object value)
            {
                properties.Add(name, value);
            }
        }
    }
}
