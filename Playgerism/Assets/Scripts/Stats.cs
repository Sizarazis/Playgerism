using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {

	// Use this for initialization
	void Start () {
        stats = new Dictionary<string, string>();

        GetStats();
        DisplayStats();
	}


    // -- VARIABLES --
    Dictionary<string, string> stats;


	// Update is called once per frame
	void Update () {
		
	}


    // TODO
    // TODO: - this reminded me that I need to set the author and title on the actual puzzle scene
    // EFFECTS: Gets the poem titles and author names and their associated best times from resources
    // MODIFIES: this
    // REQUIRES: nothing
    private void GetStats()
    {

    }


    // TODO
    // EFFECTS: Displays the stats
    // MODIFIES: this
    // REQUIRES: nothing
    private void DisplayStats()
    {

    }
}
