using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.PartLoading;
using Assets.Scripts.PartLoading.Objects;
using Assets.Scripts.Util.Resources;
using UnityEngine;

namespace Assets.Scripts.PartScripts
{
    public class AppendPartScriptScript : MonoBehaviour
    {
        public string ModuleName;
        public string PartName;

        private void Start()
        {
            if (!string.IsNullOrEmpty(ModuleName) && !string.IsNullOrEmpty(PartName))
            {
                PartTemplate template = ModuleLoader.GetPartTemplate(new ResourceLocation(ModuleName, PartName, ResourceType.PartTemplate));
                ((Part)gameObject.AddComponent(template.ScriptType)).LoadFrom(template);
            }
        }
    }
}
