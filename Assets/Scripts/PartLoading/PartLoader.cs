using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Assets.Scripts.PartLoading.Exceptions;
using Assets.Scripts.PartLoading.Objects;
using Assets.Scripts.PartScripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.PartLoading
{
    class PartLoader
    {
        private static readonly string TAG = "PartLoader";

        private static readonly string PARTS_DIR = "parts";
        private static readonly string MODELS_DIR = "models";
        private static readonly string LANG_DIR = "lang";

        private static readonly string VANILLA_MODULE_PATH = Path.Combine(Application.dataPath, "VanillaModule");
        private static readonly string VANILLA_MODULE_LANG_PATH = Path.Combine(VANILLA_MODULE_PATH, LANG_DIR);

        public static readonly PartLoader MainInstance = new PartLoader();

        private Localizer localizer = new Localizer();

        private List<Model> submodels = new List<Model>();
        private List<PartTemplate> partTemplates = new List<PartTemplate>();

        public PartLoader()
        {
            localizer.AddLang(Path.Combine(VANILLA_MODULE_LANG_PATH, "default.lang"));
            submodels.Add(loadModel("vanilla", "bullet.json"));
            partTemplates.Add(loadPartTemplate("vanilla", "cannon.json"));
        }

        public PartTemplate GetPartTemplate(string module, string name)
        {
            return partTemplates.Find(partTemplate => partTemplate.ModuleName == module && partTemplate.UnlocalizedName == name);
        }

        private Model loadModel(string module, string jsonName)
        {
            string jsonPath = Path.Combine(Path.Combine(getModulePath(module), MODELS_DIR), jsonName);
            try
            {
                return loadModel(module, JObject.Parse(File.ReadAllText(jsonPath)));
            }
            catch (IOException exception)
            {
                Log.Exception(TAG, "Unable to read JSON " + jsonPath, exception);
            }
            catch (JsonReaderException exception)
            {
                Log.Exception(TAG, "Unable to parse JSON for model", exception);
            }
            return null;
        }

        private Model loadModel(string module, JObject jObject)
        {
            string name = "JSON parsing error";
            try
            {
                name = JSONUtil.ReadProperty<string>(jObject, "name");

                float[] relativeArray = JSONUtil.ReadArrayWithDefaultValue<float>(jObject, "relative", null);
                bool isSubmodel = relativeArray == null;

                string submodelPath = JSONUtil.ReadWithDefaultValue<string>(jObject, "external_model", null);
                if (submodelPath == null)
                {
                    string texture = JSONUtil.ReadProperty<string>(jObject, "texture");
                    bool renderOnDefault = JSONUtil.ReadProperty<bool>(jObject, "render_on_default");

                    List<ModelPart> parts = new List<ModelPart>();
                    JArray jParts = JSONUtil.ReadArray(jObject, "parts");
                    foreach (JToken jToken in jParts)
                    {
                        JObject partObject = (JObject)jToken;
                        Vector2 relativePart = JSONUtil.ArrayToVector2(JSONUtil.ReadArray<float>(partObject, "relative"));
                        bool collide = JSONUtil.ReadProperty<bool>(partObject, "collide");

                        JArray jVertices = JSONUtil.ReadArray(partObject, "vertices");
                        Vector2[] vertices = new Vector2[jVertices.Count];
                        int vertexIndex = 0;
                        foreach (JToken jVertex in jVertices)
                        {
                            JArray coords = (JArray)jVertex;
                            vertices[vertexIndex] = new Vector2(Extensions.Value<float>(coords[0]), Extensions.Value<float>(coords[1]));
                            vertexIndex++;
                        }

                        JArray jJoints = JSONUtil.ReadArray(partObject, "joints");
                        int[] joints = new int[jJoints.Count];
                        int jointIndex = 0;
                        foreach (JToken jJoint in jJoints)
                        {
                            joints[jointIndex] = Extensions.Value<int>(jJoint);
                            jointIndex++;
                        }
                        parts.Add(new ModelPart(relativePart, collide, vertices, joints));
                    }
                    if (isSubmodel)
                        return new Model(module, name, texture, renderOnDefault, parts);
                    else
                        return new RenderedModel(module, name, texture, renderOnDefault, parts,
                            JSONUtil.ArrayToVector2(relativeArray));
                }
                else
                {
                    string texture = JSONUtil.ReadWithDefaultValue<string>(jObject, "texture", null);
                    bool? renderOnDefault = JSONUtil.ReadWithDefaultValue<bool?>(jObject, "render_on_default", null);

                    string[] moduleNamePair = submodelPath.Split('.');
                    if (moduleNamePair.Length == 2)
                    {
                        Model submodel = getSubmodel(moduleNamePair[0], moduleNamePair[1]);
                        if (submodel != null)
                        {
                            RenderedModel renderedModel = RenderedModel.CreateFromSubModel(submodel);
                            renderedModel.Relative = JSONUtil.ArrayToVector2(relativeArray);
                            if (texture != null)
                                renderedModel.Texture = texture;
                            if (renderOnDefault != null)
                                renderedModel.RenderOnDefault = renderOnDefault.Value;
                            return renderedModel;
                        }
                        else
                            throw new ExternalModelException(submodelPath, true);
                    }
                    else
                        throw new ExternalModelException(submodelPath, false);
                }
            }
            catch (JsonReaderException exception)
            {
                Log.Exception(TAG, "Unable to parse JSON for model", exception);
            }
            catch (PropertyReadException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON model {0}:{1}\n{2}", module, name, exception));
            }
            catch (ExternalModelException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON model {0}:{1}\n{2}", module, name, exception));
            }
            return null;
        }

        private PartTemplate loadPartTemplate(string module, string jsonName)
        {
            string jsonPath = Path.Combine(Path.Combine(getModulePath(module), PARTS_DIR), jsonName);
            try
            {
                JObject jObject = JObject.Parse(File.ReadAllText(jsonPath));

                string script = JSONUtil.ReadProperty<string>(jObject, "script");
                Type scriptType = findScriptClass(script);
                if (scriptType == null)
                    throw new ScriptNotFoundException(module, script);

                string name = JSONUtil.ReadProperty<string>(jObject, "name");
                string localizedName = localizer.GetLocalizedName(module, name);

                JObject jShopProperties = JSONUtil.ReadObject(jObject, "shop_properties");
                ShopProperties shopProperties =
                    new ShopProperties(
                        JSONUtil.ReadProperty<int>(jShopProperties, "cost"),
                        JSONUtil.ReadProperty<int>(jShopProperties, "required_level"));

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
                foreach (JToken jModelToken in jModels)
                {
                    JObject jModel = (JObject)jModelToken;
                    Model resultModel = loadModel(module, jModel);
                    if (resultModel is RenderedModel)
                        models.Add((RenderedModel)resultModel);
                    else
                    {
                        throw new PropertyReadException("relative", jModel, null, typeof(Vector2));
                    }
                }

                return new PartTemplate(module, scriptType, name, localizedName, shopProperties, scriptProperties, customScriptProperties, models);
            }
            catch (IOException exception)
            {
                Log.Exception(TAG, "Unable to read JSON " + jsonPath, exception);
            }
            catch (JsonReaderException exception)
            {
                Log.Exception(TAG, "Unable to parse JSON for part template " + jsonName, exception);
            }
            catch (PropertyReadException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON {0}:{1}\n{2}", module, jsonName, exception));
            }
            catch (UnsupportedTokenException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON {0}:{1}\n{2}", module, jsonName, exception));
            }
            catch (InvalidPropertyNameException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON {0}:{1}\n{2}", module, jsonName, exception));
            }
            catch (ScriptNotFoundException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON {0}:{1}\n{2}", module, jsonName, exception));
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

        private string getModulePath(string module)
        {
            if (module == "vanilla")
                return VANILLA_MODULE_PATH;
            else
                throw new NotSupportedException("Custom modules are not supported yet");
        }

        private Model getSubmodel(string module, string modelName)
        {
            return submodels.Find(model => model.Module == module && model.Name == modelName);
        }
    }
}
