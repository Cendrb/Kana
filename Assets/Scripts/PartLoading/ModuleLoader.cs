﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.PartLoading.Exceptions;
using Assets.Scripts.PartLoading.Objects;
using Assets.Scripts.PartScripts;
using Assets.Scripts.Util;
using Assets.Scripts.Util.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Extensions = Newtonsoft.Json.Linq.Extensions;

namespace Assets.Scripts.PartLoading
{
    public static class ModuleLoader
    {
        private static readonly string TAG = "ModuleLoader";

        private static readonly Localizer Localizer = new Localizer();
        private static readonly ModelLoader ModelLoader = new ModelLoader();
        private static readonly MaterialLoader MaterialLoader = new MaterialLoader();
        private static readonly PartTemplateLoader PartTemplateLoader = new PartTemplateLoader();

        private static readonly List<string> modules = new List<string>();

        static ModuleLoader()
        {
            modules.Add("vanilla");
            foreach (string module in modules)
            {
                foreach (ResourceLocation resourceLocation in ResourceLocation.GetResourceLocations(module, ResourceType.Lang))
                {
                    Localizer.AddLang(resourceLocation);
                }

                foreach (ResourceLocation resourceLocation in ResourceLocation.GetResourceLocations(module, ResourceType.PartTemplate))
                {
                    PartTemplateLoader.LoadResource(resourceLocation);
                }
            }
        }

        public static Material GetMaterial(ResourceLocation textureLocation)
        {
            return MaterialLoader.LoadResource(textureLocation);
        }

        public static Model LoadModel(ResourceLocation parentModelLocation, JObject jModel)
        {
            return ModelLoader.LoadModel(null, parentModelLocation, jModel);
        }

        public static PartTemplate GetPartTemplate(ResourceLocation partTemplateLocation)
        {
            return PartTemplateLoader.LoadResource(partTemplateLocation);
        }

        public static string Localize(string module, string name)
        {
            return Localizer.GetLocalizedName(module, name);
        }
    }
}
