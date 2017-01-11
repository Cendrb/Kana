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

            vehicle.Instantiate(gameObject, 1);
        }
    }
}
