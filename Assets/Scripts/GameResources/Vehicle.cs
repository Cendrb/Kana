using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ModuleResources;
using Assets.Scripts.ModuleResources.PartTemplates;
using Assets.Scripts.Util;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.GameResources
{
    public class Vehicle : IJsonSerializable
    {
        private List<PartTemplate> partTemplates = new List<PartTemplate>();
        private List<Connection> connections = new List<Connection>();

        public void Serialize(JObject targetJObject)
        {
            JArray partTemplateIdentifiers = new JArray();
            foreach (PartTemplate partTemplate in partTemplates)
            {
                partTemplateIdentifiers.Add(partTemplate.ModuleName + ":" + partTemplate.UnlocalizedName);
            }
            targetJObject.Add("part_templates", partTemplateIdentifiers);

            JArray connectionObjects = new JArray();
            foreach (Connection connection in connections)
            {
                JObject jConnection = new JObject();
                jConnection.Add("PartTemplate1Index", connection.PartTemplate1Index);
                jConnection.Add("Joint1Index", connection.Joint1Index);
                jConnection.Add("PartTemplate2Index", connection.PartTemplate2Index);
                jConnection.Add("Joint2Index", connection.Joint2Index);
                connectionObjects.Add(jConnection);
            }
            targetJObject.Add("connections", connectionObjects);
        }

        public void Deserialize(JObject sourceJObject)
        {
            JArray partTemplateIdentifiers = JSONUtil.ReadArray(sourceJObject, "part_templates");
            foreach (JToken partTemplateIdentifier in partTemplateIdentifiers)
            {
                partTemplates.Add(
                    ModuleLoader.GetPartTemplate(
                        ResourceLocation.Parse(Newtonsoft.Json.Linq.Extensions.Value<string>(partTemplateIdentifier), ResourceType.PartTemplate)));
            }

            JArray connectionObjects = JSONUtil.ReadArray(sourceJObject, "connections");
            foreach (JToken connectionObject in connectionObjects)
            {
                JObject jConnection = (JObject) connectionObject;
                Connection connection = new Connection()
                {
                    PartTemplate1Index = JSONUtil.ReadProperty<int>(jConnection, "PartTemplate1Index"),
                    Joint1Index = JSONUtil.ReadProperty<int>(jConnection, "Joint1Index"),
                    PartTemplate2Index = JSONUtil.ReadProperty<int>(jConnection, "PartTemplate2Index"),
                    Joint2Index = JSONUtil.ReadProperty<int>(jConnection, "Joint2Index")
                };
                connections.Add(connection);
            }
        }
    }
}
