using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    Line currentLine;
    Slot prevSlot;
    Slot nextSlot;

	// Update is called once per frame
	void Update () {
		
	}

    public void InstantiateSlots(int numSlots)
    {
        int yPos = -20;
        for (int i = 0; i < numSlots; i++)
        {
            yPos = yPos - 10;

            Instantiate<Slot>(this, new Vector3(0, yPos, 0), Quaternion.identity);
        }
    }

}
