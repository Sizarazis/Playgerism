using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class Utilities {
    public static int authID;
    public static int poemID;


    // EFFECTS: updates the authID and poemID to the currently selected poem
    // REQUIRES: nothing
    // MODIFIES: this
    public static void SetIDs(int currentAuth, int currentPoem)
    {
        authID = currentAuth;
        poemID = currentPoem;
    }

    // TODO: Parse a poem
    public static string[] ParsePoem()
    {     
        int poemSize = 10;
        string[] poemLines = new string[poemSize];

        // test
        poemLines[0] = "Hello";
        poemLines[1] = "This is a test";
        poemLines[2] = "And a poem";
        poemLines[3] = "I hope it works";
        poemLines[4] = "But it may not";
        poemLines[5] = "And that would be sad";
        poemLines[6] = "Devastating, in fact";
        poemLines[7] = "Therefore lets hope for the best";
        poemLines[8] = "and maybe, just maybe";
        poemLines[9] = "this will show up in the game";
        //poemLines[10] = "Hello";

        return poemLines;
    }

    // TODO: find the poem in the project
    public static string[] FindPoem()
    {
        string[] poem;
        string dir = Directory.GetCurrentDirectory();
        string path = dir + "\\Assets\\Resources\\Poems\\Authors\\" + authID + ".txt";

        poem = ParsePoem();
        return poem;
    }
}
