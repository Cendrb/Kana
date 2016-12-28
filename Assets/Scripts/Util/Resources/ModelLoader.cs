using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.PartLoading;
using Assets.Scripts.PartLoading.Exceptions;
using Assets.Scripts.PartLoading.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.Util.Resources
{
    public class ModelLoader : ResourceLoader<Model>
    {
        private static readonly string TAG = "ModelLoader";

        protected override Model loadResource(ResourceLocation resourceLocation)
        {
            string jsonPath = resourceLocation.GetPath();
            try
            {
                return LoadModel(resourceLocation, JObject.Parse(File.ReadAllText(jsonPath)));
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

        public Model LoadModel(ResourceLocation parentResourceLocation, JObject jObject)
        {
            string name = "Unable to read the name property";
            try
            {
                name = JSONUtil.ReadProperty<string>(jObject, "name");

                float[] relativeArray = JSONUtil.ReadArrayWithDefaultValue<float>(jObject, "relative", null);
                bool isStandaloneModel = relativeArray == null;

                if (isStandaloneModel && name != parentResourceLocation.Name)
                    throw new InvalidResourceNameException(parentResourceLocation, name);

                string externalModelResourceString = JSONUtil.ReadWithDefaultValue<string>(jObject, "external_model", null);
                if (externalModelResourceString == null)
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
                            vertices[vertexIndex] = new Vector2(
                                Newtonsoft.Json.Linq.Extensions.Value<float>(coords[0]),
                                Newtonsoft.Json.Linq.Extensions.Value<float>(coords[1]));
                            vertexIndex++;
                        }

                        JArray juvs = JSONUtil.ReadArray(partObject, "uvs");
                        Vector2[] uvs = new Vector2[juvs.Count];
                        int uvIndex = 0;
                        foreach (JToken juv in juvs)
                        {
                            JArray coords = (JArray)juv;
                            uvs[uvIndex] = new Vector2(Newtonsoft.Json.Linq.Extensions.Value<float>(coords[0]),
                                Newtonsoft.Json.Linq.Extensions.Value<float>(coords[1]));
                            uvIndex++;
                        }

                        JArray jJoints = JSONUtil.ReadArray(partObject, "joints");
                        int[] joints = new int[jJoints.Count];
                        int jointIndex = 0;
                        foreach (JToken jJoint in jJoints)
                        {
                            joints[jointIndex] = Newtonsoft.Json.Linq.Extensions.Value<int>(jJoint);
                            jointIndex++;
                        }
                        parts.Add(new ModelPart(relativePart, collide, vertices, uvs, joints));
                    }
                    if (isStandaloneModel)
                    {
                        Log.Info(TAG, "Successfully loaded model " + parentResourceLocation.Module + ":" + name);
                        return new Model(parentResourceLocation.Module, name, texture, renderOnDefault, parts);
                    }
                    else
                        return new RenderedModel(parentResourceLocation.Module, name, texture, renderOnDefault, parts,
                            JSONUtil.ArrayToVector2(relativeArray));
                }
                else
                {
                    string texture = JSONUtil.ReadWithDefaultValue<string>(jObject, "texture", null);
                    bool? renderOnDefault = JSONUtil.ReadWithDefaultValue<bool?>(jObject, "render_on_default", null);

                    ResourceLocation modelLocation = ResourceLocation.Parse(externalModelResourceString, ResourceType.Model);

                    Model model = LoadResource(modelLocation);
                    RenderedModel renderedModel = RenderedModel.CreateFromSubModel(model);
                    renderedModel.Relative = JSONUtil.ArrayToVector2(relativeArray);
                    if (texture != null)
                        renderedModel.Texture = texture;
                    if (renderOnDefault != null)
                        renderedModel.RenderOnDefault = renderOnDefault.Value;
                    return renderedModel;
                }
            }
            catch (PropertyReadException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON model {0}:{1}\n{2}", parentResourceLocation.Module, parentResourceLocation.Name ?? name, exception));
            }
            catch (ResourceLocationParseException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON model {0}:{1}\n{2}", parentResourceLocation.Module, parentResourceLocation.Name ?? name, exception));
            }
            catch (ResourceNotFoundException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON model {0}:{1}\n{2}", parentResourceLocation.Module, parentResourceLocation.Name ?? name, exception));
            }
            catch (InvalidResourceNameException exception)
            {
                Log.Error(TAG, string.Format("Error while processing JSON model {0}:{1}\n{2}", parentResourceLocation.Module, parentResourceLocation.Name ?? name, exception));
            }
            return null;
        }
    }
}
