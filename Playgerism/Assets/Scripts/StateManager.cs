using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        poem = GetPoem();

        numLines = poem.Length;
        numSlots = poem.Length + 1;

        BuildSlots();
        BuildLines();
        SetLineText();
    }

    public GameObject slotPrefab;
    public GameObject slots;

    public GameObject linePrefab;
    public GameObject lines;

    public string[] poem;
    public int      numLines;
    public int      numSlots;

	// Update is called once per frame
	void Update () {
		
	}

    // TODO: get a poem for the puzzle
    string[] GetPoem()
    {
        string[] lines = Utilities.ParsePoem();

        return lines;
    }

    // TODO: figure out why Unity is saying the instantiate is referencing a bunch of unrelated gameObjects
    // TODO: Display the open slots (right now its hidden behind the background)
    // EFFECTS: Instantiates slots from the slot prefab
    // MODIFIES: the slots gameObject
    // REQUIRES: slotPrefab and slots to reference a prefab and gameObject in the Unity project
    void BuildSlots()
    {
        int yPos = -20;

        for (int i = 0; i < numSlots; i++)
        {
            Quaternion rotation = slotPrefab.transform.rotation;
            Vector3 position = new Vector3(0, yPos, 0);

            Instantiate(slotPrefab, position, rotation, slots.transform);
            yPos = yPos - 10;
        }
    }

    // EFFECTS: Instantiates lines from the line prefab
    // MODIFIES: the lines gameObject
    // REQUIRES: linePrefab and lines to reference a prefab and gameObject in the Unity project
    void BuildLines()
    {
        int yPos = -20;

        for (int i = 0; i < numLines; i++)
        {
            Quaternion rotation = linePrefab.transform.rotation;
            Vector3 position = new Vector3(0, yPos, 0);

            Instantiate(linePrefab, position, rotation, lines.transform);

            yPos = yPos - 10;
        }
    }

    // EFFECTS: Sets the lines 
    // MODIFIES: the text displayed in each Lines/Line/Text gameObject
    // REQUIRES: Lines to have already instantiated its line children
    void SetLineText()
    {
        string[] scramble = ScramblePoem(poem);

        int i = 0;
        foreach(Transform line in lines.GetComponentsInChildren<Transform>())
        {
            if (line.name == "Text")
            {
                line.GetComponentInChildren<TextMesh>().text = scramble[i];
                i++;
            }
        }
    }

    // EFFECTS: Randomly shuffles an array of strings
    // MODIFIES: Nothing
    // REQUIRES: an array of strings
    string[] ScramblePoem(string[] poem)
    {
        int len = poem.Length;
        string[] outPoem = poem;

        for (int i = 0; i < len; i++)
        {
            string temp = outPoem[i];
            int random = Random.Range(i, outPoem.Length);
            outPoem[i] = outPoem[random];
            outPoem[random] = temp;
        }

        /*
        for (int i = 0; i < len; i++)
        {
            Debug.Log(outPoem[i]);
        }
        */

        return outPoem;
    }
}
