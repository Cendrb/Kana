using Assets.Scripts.PartLoading;
using Assets.Scripts.PartLoading.Objects;
using UnityEngine;

namespace Assets.Scripts.PartScripts
{
    public class Part : MonoBehaviour {

        public string UnlocalizedName { get; private set; }
        public string LocalizedName { get; private set; }
        public ShopProperties ShopProperties { get; private set; }
        public ScriptProperties ScriptProperties { get; private set; }

        public Part()
        {
        }

        public void LoadFrom(PartTemplate template)
        {
            UnlocalizedName = template.UnlocalizedName;
            LocalizedName = template.LocalizedName;
            ShopProperties = template.ShopProp; // TODO create new instance instead of linking?
            ScriptProperties = template.ScriptProp; // TODO create new instance instead of linking?
        }

        // Use this for initialization
        void Start () {
	    
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
