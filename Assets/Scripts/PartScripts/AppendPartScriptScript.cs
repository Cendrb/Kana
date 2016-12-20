using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.PartLoading;
using Assets.Scripts.PartLoading.Objects;
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
                PartTemplate template = PartLoader.MainInstance.GetPartTemplate(ModuleName, PartName);
                ((Part)gameObject.AddComponent(template.ScriptType)).LoadFrom(template);
            }
            
        }
    }
}
