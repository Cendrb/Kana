using System.Collections.Generic;
using Assets.Scripts.ModuleResources.Localization;
using Assets.Scripts.ModuleResources.Materials;
using Assets.Scripts.ModuleResources.Models;
using Assets.Scripts.ModuleResources.PartTemplates;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Extensions = Newtonsoft.Json.Linq.Extensions;

namespace Assets.Scripts.ModuleResources
{
    public static class ModuleLoader
    {
        private static readonly string TAG = "ModuleLoader";

        private static readonly Localizer Localizer = new Localizer();
        private static readonly LocalizationsLoader LocalizationsLoader = new LocalizationsLoader();
        private static readonly ModelLoader ModelLoader = new ModelLoader();
        private static readonly MaterialLoader MaterialLoader = new MaterialLoader();
        private static readonly PartTemplateLoader PartTemplateLoader = new PartTemplateLoader();

        private static readonly List<string> modules = new List<string>();

        static ModuleLoader()
        {
            string language = "default";
            modules.Add("vanilla");
            foreach (string module in modules)
            {
                Localizer.AddLang(LocalizationsLoader.LoadResource(new ResourceLocation(module, language, ResourceType.Lang)));

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

        public static List<PartTemplate> GetLoadedPartTemplates()
        {
            return PartTemplateLoader.GetAllResources();
        }

        public static string Localize(string module, string name)
        {
            return Localizer.GetLocalizedName(module, name);
        }
    }
}
