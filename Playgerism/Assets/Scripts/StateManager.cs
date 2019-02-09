using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        stringPoem = GetPoem();
        numLines = stringPoem.Length;
        numSlots = stringPoem.Length + 2;
        poem = BuildPoem();
        scrambledPoem = ScramblePoem();

        BuildSlots();
        BuildLines();
        SetLineText();
        ConnectSlots();
        AttachLinesAndSlots();
        testScore = gameObject.transform.Find("Goal Text").GetComponent<TextMesh>();
    }

    public GameObject slotPrefab;
    public GameObject slots;
    public GameObject linePrefab;
    public GameObject lines;

    public struct IDLine
    {
        public int ID;
        public string line; 
    }

    public TextMesh testScore;

    public string[] stringPoem;
    public IDLine[] poem;
    public IDLine[] scrambledPoem;

    public int      numLines;
    public int      numSlots;
    public int      yPosPoemStart = -20;
    public int      score;

    private int lineHeight =    10;

	// Update is called once per frame
	void Update () {
        CalculateScore();
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
            instSlot.GetComponent<Slot>().relPosition = i;

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
        int i = 0;
        foreach(Transform line in lines.GetComponentsInChildren<Transform>())
        {
            if (line.name == "Text")
            {
                line.GetComponentInChildren<TextMesh>().text = scrambledPoem[i].line;
                line.GetComponentInParent<Line>().lineID = scrambledPoem[i].ID;

                line.GetComponentInParent<Line>().truePlaces = new int[numLines];
                for (int j = 0; j < numLines; j++)
                {
                    line.GetComponentInParent<Line>().truePlaces[j] = -1;
                }

                //THERE IS A PROBLEM FROM HERE (IT ISN'T STORING THE CORRECT VALUE IN TRUEPLACES)
                // Find all the elements in poem[] with the same IDs as scrambledPoem[i]
                // Store their positions in poem[] in truePlaces

                int duplicateCounter = 0;
                for (int k = 0; k < numLines; k++)
                {
                    if (scrambledPoem[i].ID == poem[k].ID)
                    {
                        line.GetComponentInParent<Line>().truePlaces[duplicateCounter] = k;
                        duplicateCounter++;
                    }
                }

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
        int counter = 0;
        bool duplicateFlag = false;
        for (int i = 0; i < numLines; i++)
        {
            duplicateFlag = false;
            output[i].line = stringPoem[i];

            //Duplicate checking loop
            for (int j = 0; j < i; j++)
            {
                if (stringPoem[j] == stringPoem[i])
                {
                    duplicateFlag = true;
                    output[i].ID = output[j].ID;
                    break;
                }
            }

            if (duplicateFlag == false)
            {
                output[i].ID = counter;
                counter++;
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
        foreach (Line instLine in lines.transform.GetComponentsInChildren<Line>())
        {
            instLine.inSlot = allSlots[i];
            allSlots[i].currentLine = instLine;

            i++;
        }
    }

    // EFFECTS: Updates the score (max = 5*numLines --> 3/line for being in right slot, 1/line for each correct neighbour of that line)
    // MODIFIES: A testing UI element, will be removed
    // REQUIRES: Nothing
    void CalculateScore()
    {
      // TODO
    }
}
