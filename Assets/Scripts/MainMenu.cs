using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.PartLoading;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
    private void Start () {
	    PartLoader partLoader = new PartLoader();
        partLoader.Kana();
	}
	
	// Update is called once per frame
    private void Update () {
	    
	}
}
