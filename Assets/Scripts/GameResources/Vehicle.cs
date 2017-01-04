using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ModuleResources;
using Assets.Scripts.ModuleResources.PartTemplates;
using Assets.Scripts.PartScripts;
using Assets.Scripts.Util;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.GameResources
{
    public class Vehicle : IJsonSerializable
    {
        private List<PartTemplate> partTemplates = new List<PartTemplate>();
        private List<Connection> connections = new List<Connection>();
        private List<GameObject> partTemplateGameObjects = new List<GameObject>();

        public void Instantiate(GameObject parentGameObject)
        {
            for (int partTemplateIndex = 0; partTemplateIndex < partTemplates.Count; partTemplateIndex++)
            {
                PartTemplate partTemplate = partTemplates[partTemplateIndex];
                GameObject partGameObject = new GameObject(partTemplate.ResourceLocation.ToResourceLocationString());
                partGameObject.transform.SetParent(parentGameObject.transform, false);
                Part partScript = (Part)partGameObject.AddComponent(partTemplate.ScriptType);
                partScript.LoadFrom(partTemplate, this);
                List<Joint> joints = partScript.JointPoints;
                foreach (Connection connection in connections)
                {
                    if (connection.PartTemplate2Index == partTemplateIndex)
                    {
                        GameObject baseGameObject = partTemplateGameObjects[connection.PartTemplate1Index];
                        List<Joint> basePartJoints = baseGameObject.GetComponent<Part>().JointPoints;
                        Joint baseJoint = basePartJoints[connection.Joint1Index];
                        Joint thisJoint = joints[connection.Joint2Index];
                        Vector2 jointVectorDiff =
                            (Vector2) (Quaternion.Euler(0, 0, baseGameObject.transform.localEulerAngles.z)*baseJoint.Position) -
                            (Vector2) (thisJoint.Position);
                        Vector2 thisPosition =
                            (Vector2)baseGameObject.transform.localPosition +
                            jointVectorDiff;
                        partGameObject.transform.localPosition = thisPosition;
                        partGameObject.transform.RotateAround((Vector2)partGameObject.transform.position + thisJoint.Position, Vector3.forward, 
                            baseJoint.Rotation - (360 - baseGameObject.transform.localEulerAngles.z) + 180 - thisJoint.Rotation);
                    }
                }
                partTemplateGameObjects.Add(partGameObject);
            }
        }

        public void Serialize(JObject targetJObject)
        {
            JArray partTemplateIdentifiers = new JArray();
            foreach (PartTemplate partTemplate in partTemplates)
            {
                partTemplateIdentifiers.Add(partTemplate.ResourceLocation.ToResourceLocationString());
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
                JObject jConnection = (JObject)connectionObject;
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

        public int AddPartTemplate(PartTemplate partTemplate)
        {
            partTemplates.Add(partTemplate);
            return partTemplates.Count - 1;
        }

        public int AddConnection(Connection connection)
        {
            connections.Add(connection);
            return connections.Count - 1;
        }
    }
}
