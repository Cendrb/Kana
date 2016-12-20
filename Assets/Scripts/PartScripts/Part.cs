using UnityEngine;
using System.Collections;

public class Part : MonoBehaviour {

    public string UnlocalizedName { get; private set; }
    public string LocalizedName { get; private set; }
    public int Cost { get; private set; }
    public int RequiredLevel { get; private set; }

    public Part(string unlocalizedName, string localizedName, int cost, int requiredLevel)
    {
        UnlocalizedName = unlocalizedName;
        LocalizedName = localizedName;
        Cost = cost;
        RequiredLevel = requiredLevel;
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
