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
        private float scale;
        private List<PartTemplate> partTemplates = new List<PartTemplate>();
        private List<Connection> connections = new List<Connection>();
        private List<GameObject> partGameObjects = new List<GameObject>();
        private Dictionary<VehicleJointIdentifier, Vector2> worldJointPositions = new Dictionary<VehicleJointIdentifier, Vector2>();
        private GameObject parentGameObject = null;

        public void Instantiate(GameObject parentGameObject, float scale)
        {
            this.scale = scale;
            this.parentGameObject = parentGameObject;
            parentGameObject.transform.localScale = new Vector3(scale, scale, scale);
            for (int partTemplateIndex = 0; partTemplateIndex < partTemplates.Count; partTemplateIndex++)
            {
                PartTemplate partTemplate = partTemplates[partTemplateIndex];
                addNewPartGO(partTemplate);
            }
        }

        private int addNewPartGO(PartTemplate partTemplate)
        {
            partTemplates.Add(partTemplate);
            GameObject partGO = new GameObject(partTemplate.ResourceLocation.ToResourceLocationString());
            partGO.transform.SetParent(parentGameObject.transform, false);
            Part partScript = (Part)partGO.AddComponent(partTemplate.ScriptType);
            partScript.LoadFrom(partTemplate, this);

            partGameObjects.Add(partGO);
            if (partTemplates.Count != partGameObjects.Count)
                throw new Exception("Part templates not equal to GOs!");
            return partTemplates.Count - 1;
        }

        private void recalculatePositionsAndRotations()
        {
            for (int partGOIndex = 0; partGOIndex < partGameObjects.Count; partGOIndex++)
            {
                GameObject partGO = partGameObjects[partGOIndex];
                if (partGO != null)
                {
                    List<Joint> joints = partGO.GetComponent<Part>().JointPoints;

                    foreach (Connection connection in connections)
                    {
                        if (connection.Joint2Identifier.PartId == partGOIndex)
                        {
                            GameObject baseGameObject = partGameObjects[connection.Joint1Identifier.PartId];
                            List<Joint> basePartJoints = baseGameObject.GetComponent<Part>().JointPoints;
                            Joint baseJoint = basePartJoints[connection.Joint1Identifier.JointId];
                            Joint thisJoint = joints[connection.Joint2Identifier.JointId];
                            Vector2 jointVectorDiff =
                                (Vector2)(Quaternion.Euler(0, 0, baseGameObject.transform.eulerAngles.z) * baseJoint.Position) -
                                (Vector2)(thisJoint.Position);
                            Vector2 thisPosition =
                                (Vector2)baseGameObject.transform.position +
                                jointVectorDiff * scale;
                            partGO.transform.position = thisPosition;
                            float rotation = baseJoint.Rotation - (360 - baseGameObject.transform.eulerAngles.z) + 180 - thisJoint.Rotation;
                            partGO.transform.rotation = new Quaternion(); // reset current rotation - prevent spinning
                            partGO.transform.RotateAround((Vector2)partGO.transform.position + (thisJoint.Position * scale), Vector3.forward,
                                rotation);
                        }
                    }
                }
            }
        }

        private void recalculateWorldRelativeJointPositions()
        {
            worldJointPositions.Clear();
            for (int partGOIndex = 0; partGOIndex < partGameObjects.Count; partGOIndex++)
            {
                GameObject partGO = partGameObjects[partGOIndex];
                if (partGO != null)
                {
                    List<Joint> joints = partGO.GetComponent<Part>().JointPoints;

                    for (int jointIndex = 0; jointIndex < joints.Count; jointIndex++)
                    {
                        Joint joint = joints[jointIndex];
                        Vector2 worldRelativeJointPosition =
                            (Vector2)partGO.transform.position +
                            (Vector2)(Quaternion.Euler(0, 0, partGO.transform.eulerAngles.z) * joint.Position) * scale;
                        worldJointPositions.Add(new VehicleJointIdentifier(partGOIndex, jointIndex), worldRelativeJointPosition);
                    }
                }
            }
        }

        public VehicleJointIdentifier GetClosestToPoint(Vector2 worldPoint, int partIdToIgnore, out float closestDistance)
        {
            VehicleJointIdentifier closestJoint = null;
            closestDistance = float.MaxValue;
            foreach (KeyValuePair<VehicleJointIdentifier, Vector2> pair in worldJointPositions)
            {
                if (pair.Key.PartId != partIdToIgnore)
                {
                    float distance = Vector2.Distance(pair.Value, worldPoint);
                    if (distance < closestDistance)
                    {
                        closestJoint = pair.Key;
                        closestDistance = distance;
                    }
                }
            }
            return closestJoint;
        }

        public int AddPartTemplate(PartTemplate partTemplate, int newPartJointId, VehicleJointIdentifier targetPartIdentifier)
        {
            int id = addNewPartGO(partTemplate);
            if (targetPartIdentifier != null)
                connections.Add(new Connection(targetPartIdentifier, new VehicleJointIdentifier(id, newPartJointId)));

            Debug.Log("Printing connections:");
            foreach (Connection connection in connections)
            {
                if(connection != null)
                {
                    Debug.Log(string.Format("Connection from {0} to {1}", connection.Joint1Identifier, connection.Joint2Identifier));
                }
            }
            recalculatePositionsAndRotations();
            recalculateWorldRelativeJointPositions();
            return id;
        }

        public bool RemovePartTemplate(int index)
        {
            try
            {
                partTemplates[index] = null; // just set to null to prevent id shift
                GameObject.Destroy(partGameObjects[index]); // destroy the GO
                partGameObjects[index] = null; // just set to null to prevent id shift

                foreach (Connection connection in connections.ToArray())
                {
                    if (connection.Joint1Identifier.PartId == index || connection.Joint2Identifier.PartId == index)
                        connections.Remove(connection);
                }
                recalculateWorldRelativeJointPositions();

                return true;
            }
            catch (Exception)
            {
                Debug.LogError("Hey! This one doesn't exist anymore!");
                return false;
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
                jConnection.Add("PartTemplate1Index", connection.Joint1Identifier.PartId);
                jConnection.Add("Joint1Index", connection.Joint1Identifier.JointId);
                jConnection.Add("PartTemplate2Index", connection.Joint2Identifier.PartId);
                jConnection.Add("Joint2Index", connection.Joint2Identifier.JointId);
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
                Connection connection = new Connection(
                    new VehicleJointIdentifier(
                        JSONUtil.ReadProperty<int>(jConnection, "PartTemplate1Index"),
                        JSONUtil.ReadProperty<int>(jConnection, "Joint1Index")),
                    new VehicleJointIdentifier(
                        JSONUtil.ReadProperty<int>(jConnection, "PartTemplate2Index"),
                        JSONUtil.ReadProperty<int>(jConnection, "Joint2Index")));
            }
        }
    }
}
