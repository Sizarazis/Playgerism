using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // -- VARIABLES --
    public GameObject links;
    public GameObject menu;
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ResumeGame()
    {
        menu.SetActive(false);

        BoxCollider2D[] linkColliders = links.transform.GetComponentsInChildren<BoxCollider2D>();

        foreach (BoxCollider2D col in linkColliders)
        {
            col.enabled = true;
        }
    }
}
