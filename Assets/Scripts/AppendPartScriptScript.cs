using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ModuleResources;
using Assets.Scripts.ModuleResources.PartTemplates;
using UnityEngine;

namespace Assets.Scripts.PartScripts
{
    public class AppendPartScriptScript : MonoBehaviour
    {
        public string ModuleName;
        public string PartName;

        private void Start()
        {
            if (!string.IsNullOrEmpty(this.ModuleName) && !string.IsNullOrEmpty(this.PartName))
            {
                PartTemplate template = ModuleLoader.GetPartTemplate(new ResourceLocation(this.ModuleName, this.PartName, ResourceType.PartTemplate));
                Part.AppendNewScriptOn(template, this.gameObject).LoadFrom(template, null);
            }
        }
    }
}
