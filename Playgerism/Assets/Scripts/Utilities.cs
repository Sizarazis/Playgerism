using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

    // TODO: Parse a poem
    public static string[] ParsePoem()
    {
        string poemID = "0";
        FindPoem(poemID);

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
    static void FindPoem(string id)
    {

    }
}
