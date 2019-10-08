using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdInitialization : MonoBehaviour {


    string gameId = "3317783";
    bool testMode = true;

    // Use this for initialization
    void Start () {
        Advertisement.Initialize(gameId, testMode);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
