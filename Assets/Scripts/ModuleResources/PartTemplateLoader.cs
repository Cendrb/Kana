using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.ModuleResources.Exceptions;
using Assets.Scripts.ModuleResources.Models;
using Assets.Scripts.ModuleResources.PartTemplates;
using Assets.Scripts.ModuleResources.PartTemplates.Exceptions;
using Assets.Scripts.PartScripts;
using Assets.Scripts.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.ModuleResources
{
    public class PartTemplateLoader : ResourceLoader<PartTemplate>
    {
        private static readonly string TAG = "PartTemplateLoader";

        protected override PartTemplate loadResource(ResourceLocation resourceLocation)
        {
            string jsonPath = resourceLocation.GetPath();
            try
            {
                JObject jObject = JObject.Parse(File.ReadAllText(jsonPath));

                string script = JSONUtil.ReadProperty<string>(jObject, "script");
                Type scriptType = findScriptClass(script);
                if (scriptType == null)
                    throw new ScriptNotFoundException(resourceLocation.Module, script);

                string name = JSONUtil.ReadProperty<string>(jObject, "name");
                if (name != resourceLocation.Name)
                    throw new InvalidResourceNameException(resourceLocation, name);
                string localizedName = ModuleLoader.Localize(resourceLocation.Module, name);

                string[] tags = JSONUtil.ReadArray<string>(jObject, "tags");

                JObject jShopProperties = JSONUtil.ReadObject(jObject, "shop_properties");
                ShopProperties shopProperties =
                    new ShopProperties(
                        JSONUtil.ReadProperty<int>(jShopProperties, "Cost"),
                        JSONUtil.ReadProperty<int>(jShopProperties, "RequiredLevel"));

                JObject jScriptProperties = JSONUtil.ReadObject(jObject, "script_properties");
                ScriptProperties scriptProperties = new ScriptProperties(
                    JSONUtil.ReadProperty<float>(jScriptProperties, "Mass"),
                    JSONUtil.ReadProperty<int>(jScriptProperties, "Health"),
                    JSONUtil.ReadProperty<int>(jScriptProperties, "DamageOnTouch"));

                JObject jScriptCustomProperties = JSONUtil.ReadObject(jObject, "custom_script_properties");
                CustomScriptProperties customScriptProperties = new CustomScriptProperties();
                foreach (JProperty jProperty in jScriptCustomProperties.Properties())
                {
                    JToken jToken = jProperty.Value;
                    object value = JSONUtil.ParseJTokenToCSharpType(jToken);
                    string propertyName = jProperty.Name;
                    if (string.IsNullOrEmpty(propertyName))
                        throw new InvalidPropertyNameException(propertyName, jScriptCustomProperties);
                    customScriptProperties.AddProperty(propertyName, value);
                }

                JArray jModels = JSONUtil.ReadArray(jObject, "models");
                List<RenderedModel> models = new List<RenderedModel>();
                int modelIndex = 0;
                foreach (JToken jModelToken in jModels)
                {
                    JObject jModel = (JObject)jModelToken;
                    Model resultModel = ModuleLoader.LoadModel(resourceLocation, jModel);
                    if (resultModel == null)
                        throw new ModelsParsingException(resourceLocation, modelIndex);
                    else if (resultModel is RenderedModel)
                        models.Add((RenderedModel)resultModel);
                    else
                        throw new PropertyReadException("relative", jModel, null, typeof(Vector2));

                    modelIndex++;
                }

                Log.Info(TAG, "Successfully loaded part template " + resourceLocation.ToResourceLocationString());
                return new PartTemplate(resourceLocation.Name, scriptType, name, localizedName, tags, shopProperties, scriptProperties, customScriptProperties, models);
            }
            catch (IOException exception)
            {
                Log.Exception(TAG, "Unable to read JSON " + jsonPath, exception);
            }
            catch (JsonReaderException exception)
            {
                Log.Exception(TAG, "Unable to parse JSON for part template " + resourceLocation.ToResourceLocationString(), exception);
            }
            catch (PropertyReadException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON {0}\n{1}", resourceLocation.ToResourceLocationString(), exception));
            }
            catch (ModelsParsingException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON {0}\n{1}", resourceLocation.ToResourceLocationString(), exception));
            }
            catch (UnsupportedTokenException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON {0}\n{1}", resourceLocation.ToResourceLocationString(), exception));
            }
            catch (InvalidPropertyNameException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON {0}\n{1}", resourceLocation.ToResourceLocationString(), exception));
            }
            catch (ScriptNotFoundException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON {0}\n{1}", resourceLocation.ToResourceLocationString(), exception));
            }
            catch (InvalidResourceNameException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON {0}\n{1}", resourceLocation.ToResourceLocationString(), exception));
            }
            return null;
        }

        private Type findScriptClass(string className)
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.Name == className && type.IsSubclassOf(typeof(Part))
                    select type).FirstOrDefault();
        }
    }
}
