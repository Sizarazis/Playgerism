using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Background : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	// Use this for initialization
	void Start () {
        inBackground = false;
	}

    public bool inBackground;


	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("back down.");
        inBackground = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("back up.");
        inBackground = false;
    }
}
