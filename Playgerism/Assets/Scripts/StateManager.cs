using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        timer = 0.0f;
        minutes = 99;
        stringPoem = GetPoem();
        numLines = stringPoem.Length;
        numSlots = stringPoem.Length;
        poem = BuildPoem();
        scrambledPoem = ScramblePoem();

        BuildSlots();
        BuildLines();
        SetLineText();
        ConnectSlots();
        AttachLinesAndSlots();
        lines = GetLines();
    }

    public GameObject slotPrefab;
    public GameObject slots;
    public GameObject linePrefab;
    public GameObject linesObj;
    public Line[]     lines;

    public struct IDLine
    {
        public int ID;
        public string line;
        public bool isLast;
    }

    public string[] stringPoem;
    public IDLine[] poem;
    public IDLine[] scrambledPoem;

    public int      numLines;
    public int      numSlots;

    private int lineHeight = 10;
    private float timer;
    private int minutes;

    // Update is called once per frame
    void Update () {
        CheckLineMatches();
        UpdateLineColors();

        if (minutes < 100)
        {
            UpdateTimer();
        }
	}

    // TODO: Might need to change when implenting parsing
    // EFFECTS: Gets a poem
    // MODIFIES: this
    // REQUIRES: nothing
    string[] GetPoem()
    {
        string[] lines = Utilities.ParsePoem();

        return lines;
    }

    Line[] GetLines()
    {
        return linesObj.GetComponentsInChildren<Line>();
    }

    // EFFECTS: Instantiates slots from the slot prefab
    // MODIFIES: the slots gameObject
    // REQUIRES: slotPrefab and slots to reference a prefab and gameObject in the Unity project
    void BuildSlots()
    {
        int yPos = -25;

        for (int i = 0; i < numSlots; i++)
        {
            Quaternion rotation = slotPrefab.transform.rotation;
            Vector3 position = new Vector3(0, yPos, 1);

            GameObject instSlot = Instantiate(slotPrefab, position, rotation, slots.transform);
            instSlot.name = "Slot " + i;
            instSlot.GetComponent<Slot>().relPosition = i;

            yPos = yPos - lineHeight;
        }
    }

    // EFFECTS: Instantiates lines from the line prefab
    // MODIFIES: the lines gameObject
    // REQUIRES: linePrefab and lines to reference a prefab and gameObject in the Unity project
    void BuildLines()
    {
        int yPos = -25;

        for (int i = 0; i < numLines; i++)
        {
            Quaternion rotation = linePrefab.transform.rotation;
            Vector3 position = new Vector3(0, yPos, 0);

            GameObject instLine = Instantiate(linePrefab, position, rotation, linesObj.transform);
            instLine.name = "Line " + i;

            yPos = yPos - lineHeight;
        }
    }

    // EFFECTS: Sets the lines 
    // MODIFIES: the text displayed in each Lines/Line/Text gameObject
    // REQUIRES: Lines to have already instantiated its line children
    void SetLineText()
    {
        int i = 0;
        foreach(Transform line in linesObj.GetComponentsInChildren<Transform>())
        {
            if (line.name == "Text")
            {
                line.GetComponentInChildren<TextMesh>().text = scrambledPoem[i].line;
                line.GetComponentInParent<Line>().lineID = scrambledPoem[i].ID;
                line.GetComponentInParent<Line>().isLast = scrambledPoem[i].isLast;

                i++;
            }
        }
    }

    // EFFECTS: Builds an array of lines with IDs attached
    // MODIFIES: this
    // REQUIRES: stringPoems and numLines to be instantiated
    IDLine[] BuildPoem()
    {
        IDLine[] output = new IDLine[numLines];
        for (int i = 0; i < numLines; i++)
        {
            output[i].line = stringPoem[i];
            output[i].ID = i;
            output[i].isLast = false;

            if (i == numLines - 1)
            {
                output[i].isLast = true;
            }
        }

        return output;
    }

    // EFFECTS: Randomly shuffles the poem's lines
    // MODIFIES: nothing
    // REQUIRES: nothing
    IDLine[] ScramblePoem()
    {
        int len = poem.Length;
        IDLine[] outPoem = new IDLine[numLines];

        for (int i = 0; i < numLines; i++)
        {
            outPoem[i] = poem[i];
        }

        for (int j = 0; j < len; j++)
        {
            IDLine temp = outPoem[j];
            int random = Random.Range(j, outPoem.Length);
            outPoem[j] = outPoem[random];
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
        foreach (Line instLine in linesObj.transform.GetComponentsInChildren<Line>())
        {
            instLine.inSlot = allSlots[i];
            allSlots[i].currentLine = instLine;

            i++;
        }
    }

    // EFFECTS: Calls UpdateMatches() on every line in the poem. Ensures that each line's matches are up-to-date
    // MODIFIES: every line object in the poem
    // REQUIRES: nothing
    void CheckLineMatches()
    {
        foreach(Line line in lines)
        {
            line.UpdateMatches();
        }
    }

    // EFFECTS: Modifies the color of all lines in the poem according to how many matches they have
    // MODIFIES: the line object's background color
    // REQUIRES: the background to have a MeshRenderer
    void UpdateLineColors()
    {
        Color baseColor;
        Color green = new Color(0.23529f, 0.68627f, 0.39215f, 1);
        Color yellow = new Color(1, 1, 0.49019f, 1);
        Color outColor;

        if (lines.Length <= 0) return;
        else baseColor = lines[0].gameObject.transform.Find("Background").GetComponent<MeshRenderer>().material.color;

        foreach (Line line in lines)
        {
            int neighbours = 0;
            if (line.topMatch)
            {
                neighbours++;
            }
            if (line.bottomMatch)
            {
                neighbours++;
            }

            if (neighbours == 1)
            {
                outColor = yellow;
            }
            else if (neighbours == 2)
            {
                outColor = green;
            }
            else outColor = baseColor;

            line.gameObject.transform.Find("Background").GetComponent<MeshRenderer>().material.color = outColor;
            //line.gameObject.transform.Find("Background").GetComponent<MeshRenderer>().material.SetColor("_Color", outColor);
        }
    }

    // EFFECTS: Updates the game timer
    // MODIFIES: the in-game timer, this
    // REQUIRES: nothing
    private void UpdateTimer()
    {
        string outMin = "";
        string outSec = "";
        int seconds = 0;

        if (timer > 60)
        {
            minutes++;
            timer = 0;
        }
        else
        {
            timer = timer + Time.deltaTime;
            seconds = (int)timer;
        }

        if (minutes < 10)
        {
            outMin = "0" + minutes;
        }
        else outMin = minutes.ToString();

        if (seconds < 10)
        {
            outSec = "0" + seconds;
        }
        else outSec = seconds.ToString();

        //NOTE: THIS GAMEOBJECT WILL CHANGE NAMES
        gameObject.transform.Find("Goal Text").GetComponent<TextMesh>().text = "Timer: " + outMin + ":" + outSec;   
    }


    //NOTE: The lines should be locked in place at that point.
    //      So when we click around one, they all move
}
