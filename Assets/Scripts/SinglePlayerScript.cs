using Assets.Scripts.GameResources;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class SinglePlayerScript : MonoBehaviour
    {
        private void Start()
        {
            GameObject vehicleGO = new GameObject("Vehicle");
            Vehicle vehicle = new Vehicle();
            vehicle.Instantiate(vehicleGO, 1f);
            vehicle.Deserialize(JObject.Parse(File.ReadAllText(@"c:\temp\kana.json")));
        }
    }
}
