using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

    public Line line;
    public Slot prev;
    public Slot next;
    public int position;

    // Update is called once per frame
    void Update () {
        if (line == null)
        {
            Debug.LogWarning(this.name + ": no line associated with this slot.");
        }
    }
}
