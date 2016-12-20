using System;
using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.PartLoading;
using Assets.Scripts.PartLoading.Objects;
using UnityEngine;

namespace Assets.Scripts.PartScripts
{
    public class Part : MonoBehaviour
    {
        public string UnlocalizedName { get; private set; }
        public string LocalizedName { get; private set; }
        public ShopProperties ShopProperties { get; private set; }
        public ScriptProperties ScriptProperties { get; private set; }

        public Part()
        {
        }

        public void LoadFrom(PartTemplate template)
        {
            UnlocalizedName = template.UnlocalizedName;
            LocalizedName = template.LocalizedName;
            ShopProperties = template.ShopProp; // TODO create new instance instead of linking?
            ScriptProperties = template.ScriptProp; // TODO create new instance instead of linking?
            Type thisType = GetType();
            foreach (KeyValuePair<string, object> customScriptProperty in template.CustomScriptProp)
            {
                PropertyInfo propertyInfo = thisType.GetProperty(customScriptProperty.Key);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(this, customScriptProperty.Value, null);
                }
                else
                    Log.Error("Part#LoadFrom", string.Format("Property {0} was not found on type {1}", customScriptProperty.Key, thisType.Name));
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
