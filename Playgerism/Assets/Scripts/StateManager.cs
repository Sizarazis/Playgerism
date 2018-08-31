using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        poem = GetPoem();

        numLines = poem.Length;
        numSlots = poem.Length + 2;

        BuildSlots();
        BuildLines();
        SetLineText();
        ConnectSlots();
        AttachLinesAndSlots();
    }

    public GameObject slotPrefab;
    public GameObject slots;
    public GameObject linePrefab;
    public GameObject lines;
    public string[] poem;
    public int      numLines;
    public int      numSlots;

    private int lineHeight =    10;
    private int yPosPoemStart = -20;

	// Update is called once per frame
	void Update () {
		
	}

    // TODO: get a poem for the puzzle
    string[] GetPoem()
    {
        string[] lines = Utilities.ParsePoem();

        return lines;
    }

    // EFFECTS: Instantiates slots from the slot prefab
    // MODIFIES: the slots gameObject
    // REQUIRES: slotPrefab and slots to reference a prefab and gameObject in the Unity project
    void BuildSlots()
    {
        int yPos = yPosPoemStart;

        for (int i = 0; i < numSlots; i++)
        {
            Quaternion rotation = slotPrefab.transform.rotation;
            Vector3 position = new Vector3(0, yPos, 1);

            GameObject instSlot = Instantiate(slotPrefab, position, rotation, slots.transform);
            instSlot.name = "Slot " + i;

            yPos = yPos - lineHeight;
        }
    }

    // EFFECTS: Instantiates lines from the line prefab
    // MODIFIES: the lines gameObject
    // REQUIRES: linePrefab and lines to reference a prefab and gameObject in the Unity project
    void BuildLines()
    {
        int yPos = yPosPoemStart;

        for (int i = 0; i < numLines; i++)
        {
            Quaternion rotation = linePrefab.transform.rotation;
            Vector3 position = new Vector3(0, yPos, 0);

            GameObject instLine = Instantiate(linePrefab, position, rotation, lines.transform);
            instLine.name = "Line " + i;

            yPos = yPos - lineHeight;
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

        return outPoem;
    }

    // EFFECTS: Sets the previous and next slots for each given slot
    // MODIFIES: the Slot gameObjects
    // REQUIRES: nothing
    void ConnectSlots()
    {
        Slot prevSlot = null;
        foreach (Slot slot in slots.transform.GetComponentsInChildren<Slot>())
        {
            if (prevSlot != null)
            {
                slot.prevSlot = prevSlot;
                prevSlot.nextSlot = slot;
            }
            prevSlot = slot;
        }
    }

    // EFFECTS: Initially attaches the Lines to Slots, and Slots to Lines
    // MODIFIES: the Line and Slot gameObjects
    // REQUIRES: nothing
    void AttachLinesAndSlots()
    {
        Slot[] allSlots = slots.transform.GetComponentsInChildren<Slot>();

        int i = 0;
        foreach (Line instLine in lines.transform.GetComponentsInChildren<Line>())
        {
            instLine.inSlot = allSlots[i];
            allSlots[i].currentLine = instLine;

            i++;
        }
    }
}
