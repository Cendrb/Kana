using UnityEngine;
using System.Collections;
using Assets.Scripts.ModuleResources.PartTemplates;
using System.Collections.Generic;
using Assets.Scripts.ModuleResources;
using Assets.Scripts.PartScripts;
using Assets.Scripts.GameResources;
using Joint = Assets.Scripts.GameResources.Joint;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

public class GarageScript : MonoBehaviour
{
    private const int ONE_ITEM_WIDTH = 100;
    private const float STICKY_JOINTY_DISTANCE = 0.2f;
    private const float MENU_ITEMS_SCALE = 15;

    public GameObject partsMenuGO;

    private List<PartTemplate> partTemplates;

    private GameObject draggedGO = null;
    private int temporarilyAddedPartTemplateIndex = -1;
    private Vector2 draggedGOOffset = new Vector2();
    private float lastForeignJointDistance = float.MaxValue;

    private Vehicle vehicle;

    private void Start()
    {
        Canvas canvas = this.gameObject.GetComponent<Canvas>();
        GameObject vehicleGO = new GameObject("Vehicle");
        Transform mainTransform = vehicleGO.transform;
        //mainTransform.SetParent(gameObject.transform, false);
        mainTransform.localPosition = new Vector2(canvas.pixelRect.width / canvas.referencePixelsPerUnit / 2, canvas.pixelRect.height / canvas.referencePixelsPerUnit / 2);
        //mainTransform.localScale = new Vector2(1f, 1f);

        this.vehicle = new Vehicle();
        this.vehicle.Instantiate(vehicleGO, 0.3f);
        this.vehicle.AppendNewPartTemplate(ModuleLoader.GetPartTemplate(new ResourceLocation("vanilla", "hull", ResourceType.PartTemplate)), 0, null);

        this.partTemplates = ModuleLoader.GetLoadedPartTemplates();
        int partIndex = 0;
        RectTransform parentTransform = this.partsMenuGO.GetComponent<RectTransform>();
        foreach (PartTemplate template in this.partTemplates)
        {
            GameObject itemInShopGO = new GameObject(template.ResourceLocation.ToResourceLocationString());
            Part.AppendNewScriptOn(template, itemInShopGO).LoadFrom(template, null);
            Transform transform = itemInShopGO.transform;
            //transform.anchorMin = new Vector2(0, 1);
            //transform.anchorMax = new Vector2(0, 1);
            //transform.anchoredPosition3D = new Vector3(partIndex * ONE_ITEM_WIDTH, -parentTransform.rect.height, -10);
            transform.localPosition = new Vector3(partIndex * ONE_ITEM_WIDTH, -parentTransform.rect.height, -10);
            transform.SetParent(parentTransform, false);
            transform.localScale = new Vector2(MENU_ITEMS_SCALE, MENU_ITEMS_SCALE);
            partIndex++;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (this.draggedGO != null)
            {
                this.draggedGO.transform.RotateAround((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 90);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            using (StreamWriter writer = File.CreateText(@"c:\temp\kana.json"))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JObject jObject = new JObject();
                this.vehicle.Serialize(jObject);
                jObject.WriteTo(jsonWriter);
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            this.vehicle.Deserialize(JObject.Parse(File.ReadAllText(@"c:\temp\kana.json")));
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 vector2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(vector2, Vector2.zero);
            if (hit.collider != null)
            {
                GameObject partGameObject = hit.collider.gameObject.transform.parent.gameObject; // go to the parent
                Part partScript = partGameObject.GetComponent<Part>();
                if (partScript != null)
                {
                    PartTemplate partTemplate = ModuleLoader.GetPartTemplate(partScript.ResourceLocation);
                    if (partScript.IsPartOfVehicle())
                    {
                        int partIndex = this.vehicle.GetIndex(partGameObject);
                        if (this.vehicle.GetConnections(partIndex).Count <= 1)
                        {
                            this.draggedGO = new GameObject();
                            Part.AppendNewScriptOn(partTemplate, this.draggedGO).LoadFrom(partTemplate, null);
                            this.draggedGO.transform.SetParent(this.partsMenuGO.transform, true);
                            this.draggedGO.transform.position = partGameObject.transform.position;
                            this.draggedGO.transform.localScale = new Vector2(MENU_ITEMS_SCALE, MENU_ITEMS_SCALE);
                            this.draggedGOOffset = hit.point - (Vector2)this.draggedGO.transform.position;
                            this.vehicle.RemovePartTemplate(partIndex);
                        }
                    }
                    else
                    {
                        Debug.Log("Selected " + partScript.ResourceLocation.ToResourceLocationString());
                        this.draggedGO = new GameObject();
                        Part.AppendNewScriptOn(partTemplate, this.draggedGO).LoadFrom(partTemplate, null);
                        this.draggedGO.transform.SetParent(this.partsMenuGO.transform, true);
                        this.draggedGO.transform.position = partGameObject.transform.position;
                        this.draggedGO.transform.localScale = new Vector2(MENU_ITEMS_SCALE, MENU_ITEMS_SCALE);
                        this.draggedGOOffset = hit.point - (Vector2)this.draggedGO.transform.position;
                    }
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (this.draggedGO != null)
            {
                Vector2 lastDraggedGOPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.draggedGOOffset;
                VehicleJointIdentifier closestForeignJointInRange = null;
                float closestDistance = STICKY_JOINTY_DISTANCE;
                int ourJointId = -1;
                List<Joint> thisJoints = this.draggedGO.GetComponent<Part>().JointPoints;
                for (int jointIndex = 0; jointIndex < thisJoints.Count; jointIndex++)
                {
                    Joint joint = thisJoints[jointIndex];
                    Vector2 worldRelativeJointPosition =
                        (Vector2)lastDraggedGOPosition +
                        (Vector2)(Quaternion.Euler(0, 0, this.draggedGO.transform.eulerAngles.z) * joint.Position) * this.draggedGO.transform.lossyScale.x;
                    float distance;
                    VehicleJointIdentifier jointResult = this.vehicle.GetClosestToPoint(worldRelativeJointPosition, this.temporarilyAddedPartTemplateIndex, out distance);
                    if (jointResult != null && distance < closestDistance)
                    {
                        closestForeignJointInRange = jointResult;
                        closestDistance = distance;
                        ourJointId = jointIndex;
                    }
                }
                if (closestForeignJointInRange != null)
                {
                    if (this.temporarilyAddedPartTemplateIndex == -1 || (closestDistance < this.lastForeignJointDistance && this.temporarilyAddedPartTemplateIndex != -1))
                    {
                        if (this.temporarilyAddedPartTemplateIndex != -1)
                        {
                            this.vehicle.RemovePartTemplate(this.temporarilyAddedPartTemplateIndex);
                        }
                        else
                        {
                            this.draggedGO.SetActive(false);
                        }
                        this.temporarilyAddedPartTemplateIndex = this.vehicle.AppendNewPartTemplate(ModuleLoader.GetPartTemplate(this.draggedGO.GetComponent<Part>().ResourceLocation), ourJointId, closestForeignJointInRange);
                    }
                    this.lastForeignJointDistance = closestDistance;
                }
                else
                {
                    if (this.temporarilyAddedPartTemplateIndex != -1)
                    {
                        this.draggedGO.SetActive(true);
                        this.vehicle.RemovePartTemplate(this.temporarilyAddedPartTemplateIndex);
                        this.temporarilyAddedPartTemplateIndex = -1;
                    }
                }
                this.draggedGO.transform.position = lastDraggedGOPosition;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (this.temporarilyAddedPartTemplateIndex != -1)
            {
                this.temporarilyAddedPartTemplateIndex = -1;
            }

            GameObject.Destroy(this.draggedGO);
            this.draggedGO = null;
        }
    }
}
