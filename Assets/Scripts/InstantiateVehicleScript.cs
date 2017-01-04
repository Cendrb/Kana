using Assets.Scripts.GameResources;
using Assets.Scripts.ModuleResources;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts
{
    class InstantiateVehicleScript : MonoBehaviour
    {
        private void Start()
        {
            Vehicle vehicle = new Vehicle();
            vehicle.AddPartTemplate(
                ModuleLoader.GetPartTemplate(new ResourceLocation("vanilla", "hull", ResourceType.PartTemplate)));
            vehicle.AddPartTemplate(
                ModuleLoader.GetPartTemplate(new ResourceLocation("vanilla", "hull", ResourceType.PartTemplate)));
            vehicle.AddPartTemplate(
                ModuleLoader.GetPartTemplate(new ResourceLocation("vanilla", "cannon", ResourceType.PartTemplate)));
            vehicle.AddPartTemplate(
                ModuleLoader.GetPartTemplate(new ResourceLocation("vanilla", "cannon", ResourceType.PartTemplate)));
            vehicle.AddPartTemplate(
                ModuleLoader.GetPartTemplate(new ResourceLocation("vanilla", "cannon", ResourceType.PartTemplate)));
            vehicle.AddPartTemplate(
                ModuleLoader.GetPartTemplate(new ResourceLocation("vanilla", "cannon", ResourceType.PartTemplate)));
            vehicle.AddPartTemplate(
                ModuleLoader.GetPartTemplate(new ResourceLocation("vanilla", "hull", ResourceType.PartTemplate)));
            vehicle.AddConnection(new Connection(0, 0, 1, 2));
            vehicle.AddConnection(new Connection(1, 3, 2, 0));
            vehicle.AddConnection(new Connection(0, 2, 3, 0));
            vehicle.AddConnection(new Connection(3, 2, 4, 0));
            vehicle.AddConnection(new Connection(4, 2, 5, 0));
            vehicle.AddConnection(new Connection(4, 6, 6, 0));

            vehicle.Instantiate(gameObject);

            Vector2 baseVector = new Vector2(0, 1);
            Vector2 topVector = new Vector2(1, 0);
            Vector2 bottomVector = new Vector2(-1, 0);
            float angleTop = VectorUtil.AngleBetweenVector2(baseVector, topVector);
            float angleBottom = VectorUtil.AngleBetweenVector2(baseVector, bottomVector);
        }
    }
}
