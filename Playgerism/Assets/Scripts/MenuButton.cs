using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // -- VARIABLES --
    public GameObject links;
    public GameObject menu;
    public Transform button;

	// Update is called once per frame
	void Update () {
		if (menu.activeInHierarchy)
        {
            button.GetComponent<Button>().interactable = false;
        }
        else
        {
            button.GetComponent<Button>().interactable = true;
        }

	}

    public void OpenMenu()
    {
        menu.SetActive(true);
        BoxCollider2D[] linkColliders = links.transform.GetComponentsInChildren<BoxCollider2D>();

        foreach (BoxCollider2D col in linkColliders)
        {
            col.enabled = false;
        }
    }
}
