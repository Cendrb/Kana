using System;
using System.Collections.Generic;
using Assets.Scripts.ModuleResources.Models;

namespace Assets.Scripts.ModuleResources.PartTemplates
{
    public class PartTemplate
    {
        public ResourceLocation ResourceLocation { get; private set; }
        public Type ScriptType { get; private set; }
        public string LocalizedName { get; private set; }
        public string[] Tags { get; private set; }

        public ShopProperties ShopProp { get; private set; }
        public ScriptProperties ScriptProp { get; private set; }
        public CustomScriptProperties CustomScriptProp { get; private set; }

        public List<RenderedModel> Models { get; private set; }

        public PartTemplate(ResourceLocation resourceLocation, Type scriptType, string localizedName, string[] tags, ShopProperties shopProperties, ScriptProperties scriptProperties, CustomScriptProperties customScriptProperties, List<RenderedModel> models)
        {
            ResourceLocation = resourceLocation;
            ScriptType = scriptType;
            LocalizedName = localizedName;
            Tags = tags;
            ShopProp = shopProperties;
            ScriptProp = scriptProperties;
            CustomScriptProp = customScriptProperties;
            Models = models;
        }
    }
}
