using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        protected event Action<Vehicle> PartsChanged = delegate { };

        private float scale;
        private List<PartTemplate> partTemplates = new List<PartTemplate>();
        private List<Connection> connections = new List<Connection>();
        private List<GameObject> partGameObjects = new List<GameObject>();
        private Dictionary<string, List<Part>> taggedPartScriptReferences = new Dictionary<string, List<Part>>();
        private Dictionary<VehicleJointIdentifier, Vector2> worldJointPositions = new Dictionary<VehicleJointIdentifier, Vector2>();
        private GameObject parentGameObject = null;

        public void Instantiate(GameObject parentGameObject, float scale)
        {
            this.scale = scale;
            this.parentGameObject = parentGameObject;
            parentGameObject.transform.localScale = new Vector3(scale, scale, scale);
            for (int partTemplateIndex = 0; partTemplateIndex < this.partTemplates.Count; partTemplateIndex++)
            {
                PartTemplate partTemplate = this.partTemplates[partTemplateIndex];
                AddNewPartGO(partTemplate);
            }

            PartsChanged(this);
        }

        private int AddNewPartGO(PartTemplate partTemplate)
        {
            this.partTemplates.Add(partTemplate);
            GameObject partGO = new GameObject(partTemplate.ResourceLocation.ToResourceLocationString());
            partGO.transform.SetParent(this.parentGameObject.transform, false);
            Part partScript = Part.AppendNewScriptOn(partTemplate, partGO);
            partScript.LoadFrom(partTemplate, this);

            this.partGameObjects.Add(partGO);

            RecalculateTaggedParts();

            PartsChanged(this);

            if (this.partTemplates.Count != this.partGameObjects.Count)
            {
                throw new Exception("Part templates not equal to GOs!");
            }

            return this.partTemplates.Count - 1;
        }

        private void RecalculatePositionsAndRotations()
        {
            for (int partGOIndex = 0; partGOIndex < this.partGameObjects.Count; partGOIndex++)
            {
                GameObject partGO = this.partGameObjects[partGOIndex];
                List<Joint> joints = partGO.GetComponent<Part>().JointPoints;

                foreach (Connection connection in this.connections)
                {
                    if (connection.Joint2Identifier.PartId == partGOIndex)
                    {
                        GameObject baseGameObject = this.partGameObjects[connection.Joint1Identifier.PartId];
                        List<Joint> basePartJoints = baseGameObject.GetComponent<Part>().JointPoints;
                        Joint baseJoint = basePartJoints[connection.Joint1Identifier.JointId];
                        Joint thisJoint = joints[connection.Joint2Identifier.JointId];
                        Vector2 jointVectorDiff =
                            (Vector2)(Quaternion.Euler(0, 0, baseGameObject.transform.eulerAngles.z) * baseJoint.Position) -
                            (Vector2)(thisJoint.Position);
                        Vector2 thisPosition =
                            (Vector2)baseGameObject.transform.position +
                            jointVectorDiff * this.scale;
                        partGO.transform.position = thisPosition;
                        float rotation = baseJoint.Rotation - (360 - baseGameObject.transform.eulerAngles.z) + 180 - thisJoint.Rotation;
                        partGO.transform.rotation = new Quaternion(); // reset current rotation - prevent spinning
                        partGO.transform.RotateAround((Vector2)partGO.transform.position + (thisJoint.Position * this.scale), Vector3.forward,
                            rotation);
                    }
                }
            }
        }

        private void RecalculateWorldRelativeJointPositions()
        {
            this.worldJointPositions.Clear();
            for (int partGOIndex = 0; partGOIndex < this.partGameObjects.Count; partGOIndex++)
            {
                GameObject partGO = this.partGameObjects[partGOIndex];
                List<Joint> joints = partGO.GetComponent<Part>().JointPoints;

                for (int jointIndex = 0; jointIndex < joints.Count; jointIndex++)
                {
                    Joint joint = joints[jointIndex];
                    Vector2 worldRelativeJointPosition =
                        (Vector2)partGO.transform.position +
                        (Vector2)(Quaternion.Euler(0, 0, partGO.transform.eulerAngles.z) * joint.Position) * this.scale;
                    this.worldJointPositions.Add(new VehicleJointIdentifier(partGOIndex, jointIndex), worldRelativeJointPosition);
                }
            }
        }

        private void RecalculateTaggedParts()
        {
            this.taggedPartScriptReferences.Clear();

            for (int index = 0; index < this.partTemplates.Count; index++)
            {
                PartTemplate template = this.partTemplates[index];
                foreach (string tag in template.Tags)
                {
                    if (!this.taggedPartScriptReferences.ContainsKey(tag))
                    {
                        this.taggedPartScriptReferences.Add(tag, new List<Part>());
                    }

                    this.taggedPartScriptReferences[tag].Add(this.partGameObjects[index].GetComponent<Part>());
                }

            }
        }

        public List<Part> GetAllScriptsWithTag(string tag)
        {
            return new List<Part>(this.taggedPartScriptReferences[tag]);
        }

        public int GetIndex(GameObject partGO)
        {
            return this.partGameObjects.IndexOf(partGO);
        }

        public int GetIndex(PartTemplate partTemplate)
        {
            return this.partTemplates.IndexOf(partTemplate);
        }

        public List<Connection> GetConnections(int partIndex)
        {
            return this.connections.Where(connection => connection.Joint1Identifier.PartId == partIndex || connection.Joint2Identifier.PartId == partIndex).ToList();
        }

        public VehicleJointIdentifier GetClosestToPoint(Vector2 worldPoint, int partIdToIgnore, out float closestDistance)
        {
            VehicleJointIdentifier closestJoint = null;
            closestDistance = float.MaxValue;
            foreach (KeyValuePair<VehicleJointIdentifier, Vector2> pair in this.worldJointPositions)
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

        public int AppendNewPartTemplate(PartTemplate partTemplate, int newPartJointId, VehicleJointIdentifier targetPartIdentifier)
        {
            int id = AddNewPartGO(partTemplate);
            if (targetPartIdentifier != null)
            {
                this.connections.Add(new Connection(targetPartIdentifier, new VehicleJointIdentifier(id, newPartJointId)));
            }

            RecalculatePositionsAndRotations();
            RecalculateWorldRelativeJointPositions();
            return id;
        }

        public bool RemovePartTemplate(int index)
        {
            try
            {
                this.partTemplates.RemoveAt(index);
                GameObject.Destroy(this.partGameObjects[index]); // destroy the GO
                this.partGameObjects.RemoveAt(index);

                foreach (Connection connection in this.connections.ToArray())
                {
                    if (connection.Joint1Identifier.PartId == index || connection.Joint2Identifier.PartId == index)
                    {
                        this.connections.Remove(connection);
                    }

                    if (connection.Joint1Identifier.PartId > index) // shift ids
                    {
                        connection.Joint1Identifier.PartId--;
                    }

                    if (connection.Joint2Identifier.PartId > index) // shift ids
                    {
                        connection.Joint2Identifier.PartId--;
                    }
                }

                RecalculateTaggedParts();

                PartsChanged(this);

                RecalculateWorldRelativeJointPositions();

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
            foreach (PartTemplate partTemplate in this.partTemplates)
            {
                partTemplateIdentifiers.Add(partTemplate.ResourceLocation.ToResourceLocationString());
            }
            targetJObject.Add("part_templates", partTemplateIdentifiers);

            JArray connectionObjects = new JArray();
            foreach (Connection connection in this.connections)
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
            for (int partTemplateIndex = 0; partTemplateIndex < this.partTemplates.Count; partTemplateIndex++)
            {
                RemovePartTemplate(partTemplateIndex);
            }

            this.partTemplates.Clear();
            this.partGameObjects.Clear();
            this.connections.Clear();

            JArray partTemplateIdentifiers = JSONUtil.ReadArray(sourceJObject, "part_templates");
            foreach (JToken partTemplateIdentifier in partTemplateIdentifiers)
            {
                AddNewPartGO(
                    ModuleLoader.GetPartTemplate(
                        ResourceLocation.Parse(Newtonsoft.Json.Linq.Extensions.Value<string>(partTemplateIdentifier), ResourceType.PartTemplate)));
            }

            JArray connectionObjects = JSONUtil.ReadArray(sourceJObject, "connections");
            foreach (JToken connectionObject in connectionObjects)
            {
                JObject jConnection = (JObject)connectionObject;
                this.connections.Add(new Connection(
                    new VehicleJointIdentifier(
                        JSONUtil.ReadProperty<int>(jConnection, "PartTemplate1Index"),
                        JSONUtil.ReadProperty<int>(jConnection, "Joint1Index")),
                    new VehicleJointIdentifier(
                        JSONUtil.ReadProperty<int>(jConnection, "PartTemplate2Index"),
                        JSONUtil.ReadProperty<int>(jConnection, "Joint2Index"))));
            }

            RecalculatePositionsAndRotations();
            RecalculateWorldRelativeJointPositions();
        }
    }
}
