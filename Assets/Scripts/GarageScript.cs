using UnityEngine;
using System.Collections;
using Assets.Scripts.ModuleResources.PartTemplates;
using System.Collections.Generic;
using Assets.Scripts.ModuleResources;
using Assets.Scripts.PartScripts;
using Assets.Scripts.GameResources;
using Joint = Assets.Scripts.GameResources.Joint;

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
        Canvas canvas = gameObject.GetComponent<Canvas>();
        GameObject vehicleGO = new GameObject("Vehicle");
        Transform mainTransform = vehicleGO.transform;
        //mainTransform.SetParent(gameObject.transform, false);
        mainTransform.localPosition = new Vector2(canvas.pixelRect.width / canvas.referencePixelsPerUnit / 2, canvas.pixelRect.height / canvas.referencePixelsPerUnit / 2);
        //mainTransform.localScale = new Vector2(1f, 1f);

        vehicle = new Vehicle();
        vehicle.Instantiate(vehicleGO, 0.3f);
        vehicle.AppendNewPartTemplate(ModuleLoader.GetPartTemplate(new ResourceLocation("vanilla", "hull", ResourceType.PartTemplate)), 0, null);

        partTemplates = ModuleLoader.GetLoadedPartTemplates();
        int partIndex = 0;
        RectTransform parentTransform = partsMenuGO.GetComponent<RectTransform>();
        foreach (PartTemplate template in partTemplates)
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
            if (draggedGO != null)
            {
                draggedGO.transform.RotateAround((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 90);
            }
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
                        int partIndex = vehicle.GetIndex(partGameObject);
                        if(vehicle.GetConnections(partIndex).Count <= 1)
                        {
                            draggedGO = new GameObject();
                            Part.AppendNewScriptOn(partTemplate, draggedGO).LoadFrom(partTemplate, null);
                            draggedGO.transform.SetParent(partsMenuGO.transform, true);
                            draggedGO.transform.position = partGameObject.transform.position;
                            draggedGO.transform.localScale = new Vector2(MENU_ITEMS_SCALE, MENU_ITEMS_SCALE);
                            draggedGOOffset = hit.point - (Vector2)draggedGO.transform.position;
                            vehicle.RemovePartTemplate(partIndex);
                        }
                    }
                    else
                    {
                        Debug.Log("Selected " + partScript.ResourceLocation.ToResourceLocationString());
                        draggedGO = new GameObject();
                        Part.AppendNewScriptOn(partTemplate, draggedGO).LoadFrom(partTemplate, null);
                        draggedGO.transform.SetParent(partsMenuGO.transform, true);
                        draggedGO.transform.position = partGameObject.transform.position;
                        draggedGO.transform.localScale = new Vector2(MENU_ITEMS_SCALE, MENU_ITEMS_SCALE);
                        draggedGOOffset = hit.point - (Vector2)draggedGO.transform.position;
                    }
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (draggedGO != null)
            {
                Vector2 lastDraggedGOPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - draggedGOOffset;
                VehicleJointIdentifier closestForeignJointInRange = null;
                float closestDistance = STICKY_JOINTY_DISTANCE;
                int ourJointId = -1;
                List<Joint> thisJoints = draggedGO.GetComponent<Part>().JointPoints;
                for (int jointIndex = 0; jointIndex < thisJoints.Count; jointIndex++)
                {
                    Joint joint = thisJoints[jointIndex];
                    Vector2 worldRelativeJointPosition =
                        (Vector2)lastDraggedGOPosition +
                        (Vector2)(Quaternion.Euler(0, 0, draggedGO.transform.eulerAngles.z) * joint.Position) * draggedGO.transform.lossyScale.x;
                    float distance;
                    VehicleJointIdentifier jointResult = vehicle.GetClosestToPoint(worldRelativeJointPosition, temporarilyAddedPartTemplateIndex, out distance);
                    if (jointResult != null && distance < closestDistance)
                    {
                        closestForeignJointInRange = jointResult;
                        closestDistance = distance;
                        ourJointId = jointIndex;
                    }
                }
                if (closestForeignJointInRange != null)
                {
                    if (temporarilyAddedPartTemplateIndex == -1 || (closestDistance < lastForeignJointDistance && temporarilyAddedPartTemplateIndex != -1))
                    {
                        if (temporarilyAddedPartTemplateIndex != -1)
                        {
                            vehicle.RemovePartTemplate(temporarilyAddedPartTemplateIndex);
                        }
                        else
                        {
                            draggedGO.SetActive(false);
                        }
                        temporarilyAddedPartTemplateIndex = vehicle.AppendNewPartTemplate(ModuleLoader.GetPartTemplate(draggedGO.GetComponent<Part>().ResourceLocation), ourJointId, closestForeignJointInRange);
                    }
                    lastForeignJointDistance = closestDistance;
                }
                else
                {
                    if (temporarilyAddedPartTemplateIndex != -1)
                    {
                        draggedGO.SetActive(true);
                        vehicle.RemovePartTemplate(temporarilyAddedPartTemplateIndex);
                        temporarilyAddedPartTemplateIndex = -1;
                    }
                }
                draggedGO.transform.position = lastDraggedGOPosition;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (temporarilyAddedPartTemplateIndex != -1)
                temporarilyAddedPartTemplateIndex = -1;
            GameObject.Destroy(draggedGO);
            draggedGO = null;
        }
    }
}
