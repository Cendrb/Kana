using Assets.Scripts.GameResources;
using Assets.Scripts.ModuleResources;
using UnityEngine;

namespace Assets.Scripts
{
    class InstantiateVehicleScript : MonoBehaviour
    {
        private void Start()
        {
            Vehicle vehicle = new Vehicle();
            vehicle.AddPartTemplate(
                ModuleLoader.GetPartTemplate(new ResourceLocation("vanilla", "cannon", ResourceType.PartTemplate)));
            vehicle.Instantiate(gameObject);
        }
    }
}
