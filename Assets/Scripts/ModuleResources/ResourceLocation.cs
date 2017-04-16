using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.ModuleResources.Exceptions;
using UnityEngine;

namespace Assets.Scripts.ModuleResources
{
    public enum ResourceType { Model, PartTemplate, Texture, Lang }

    public class ResourceLocation
    {
        private static readonly string VANILLA_MODULE_PATH = Path.Combine(Application.dataPath, "VanillaModule");
        private static readonly char MODULE_NAME_SEPARATOR = ':';

        public string Module { get; set; }
        public string Name { get; set; }
        public ResourceType Type { get; set; }

        public ResourceLocation(string module, string name, ResourceType type)
        {
            this.Module = module;
            this.Name = name;
            this.Type = type;
        }

        public string GetPath()
        {
            return Path.Combine(getModulePath(this.Module), Path.Combine(getResourceTypeFolder(this.Type), this.Name + getResourceTypeExtension(this.Type)));
        }

        public bool FileExists()
        {
            return File.Exists(GetPath());
        }

        public string ToResourceLocationString()
        {
            return this.Module + MODULE_NAME_SEPARATOR + this.Name;
        }

        public override string ToString()
        {
            return ToResourceLocationString();
        }

        public static List<ResourceLocation> GetResourceLocations(string module, ResourceType type)
        {
            List<ResourceLocation> resourceLocations = new List<ResourceLocation>();
            string requiredExtension = getResourceTypeExtension(type);
            string langsPath = Path.Combine(getModulePath(module), getResourceTypeFolder(type));
            foreach (string langPath in Directory.GetFiles(langsPath))
            {
                if (Path.GetExtension(langPath) == requiredExtension)
                {
                    resourceLocations.Add(new ResourceLocation(module, Path.GetFileNameWithoutExtension(langPath), type));
                }
            }
            return resourceLocations;
        }

        public static ResourceLocation Parse(string resourceLocationString, ResourceType type)
        {
            string[] moduleNamePair = resourceLocationString.Split(MODULE_NAME_SEPARATOR);
            if (moduleNamePair.Length == 2)
            {
                return new ResourceLocation(moduleNamePair[0], moduleNamePair[1], type);
            }
            else
            {
                throw new ResourceLocationParseException(resourceLocationString);
            }
        }

        private static string getResourceTypeFolder(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Model:
                    return "models";
                case ResourceType.PartTemplate:
                    return "part_templates";
                case ResourceType.Texture:
                    return "textures";
                case ResourceType.Lang:
                    return "lang";
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        private static string getResourceTypeExtension(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Model:
                    return ".json";
                case ResourceType.PartTemplate:
                    return ".json";
                case ResourceType.Texture:
                    return ".png";
                case ResourceType.Lang:
                    return ".lang";
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        private static string getModulePath(string module)
        {
            if (module == "vanilla")
            {
                return VANILLA_MODULE_PATH;
            }
            else
            {
                throw new NotSupportedException("Custom modules are not supported yet");
            }
        }
    }
}
