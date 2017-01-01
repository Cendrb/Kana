using System;
using System.Collections.Generic;
using Assets.Scripts.ModuleResources.Models;

namespace Assets.Scripts.ModuleResources.PartTemplates
{
    public class PartTemplate
    {
        public string ModuleName { get; private set; }
        public Type ScriptType { get; private set; }
        public string UnlocalizedName { get; private set; }
        public string LocalizedName { get; private set; }
        public string[] Tags { get; private set; }

        public ShopProperties ShopProp { get; private set; }
        public ScriptProperties ScriptProp { get; private set; }
        public CustomScriptProperties CustomScriptProp { get; private set; }

        public List<RenderedModel> Models { get; private set; }

        public PartTemplate(string moduleName, Type scriptType, string unlocalizedName, string localizedName, string[] tags, ShopProperties shopProperties, ScriptProperties scriptProperties, CustomScriptProperties customScriptProperties, List<RenderedModel> models)
        {
            ModuleName = moduleName;
            ScriptType = scriptType;
            UnlocalizedName = unlocalizedName;
            LocalizedName = localizedName;
            Tags = tags;
            ShopProp = shopProperties;
            ScriptProp = scriptProperties;
            CustomScriptProp = customScriptProperties;
            Models = models;
        }
    }
}
