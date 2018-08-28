using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        poem = GetPoem();

        numLines = poem.Length;
        numSlots = poem.Length + 1;

        slots = new Slot[numSlots];
        BuildSlots();
        BuildLines();
	}

    public string[] poem;
    public int      numLines;
    public int      numSlots;
    public Slot[]   slots;

	// Update is called once per frame
	void Update () {
		
	}

    // TODO: get a poem for the puzzle
    string[] GetPoem()
    {
        string[] lines = Utilities.ParsePoem();

        return lines;
    }

    // Shuffles an array of strings
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

        for (int i = 0; i < len; i++)
        {
            Debug.Log(outPoem[i]);
        }

        return outPoem;
    }

    // TODO: Generate the slots for the poem
    // need to change the location of the slots
    void BuildSlots()
    {
        
    }

    // TODO: Generate the lines for the poem
    // need to change the text and the location of the lines
    void BuildLines()
    {
        string[] scrambledPoem = ScramblePoem(poem);
    }
}
