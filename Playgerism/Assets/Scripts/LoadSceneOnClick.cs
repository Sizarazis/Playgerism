using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LoadSceneOnClick : MonoBehaviour {

    public void LoadByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }



    // EFFECTS: Chooses a random poem to open
    // MODIFIES: Utilities
    // REQUIRES: nothing
    public void LoadRandom(int index)
    {
        // Get random author
        // Get random poem from author

        string dir = Directory.GetCurrentDirectory();
        string path = dir + "\\Assets\\Resources\\Poems\\AuthIDs.csv";
        string[] lines = File.ReadAllLines(path);

        int randomAuth = Random.Range(1, lines.Length);
        Utilities.authID = randomAuth - 1;

        string authPath = dir + "\\Assets\\Resources\\Poems\\Authors\\" + Utilities.authID + ".txt";
        string[] authLines = File.ReadAllLines(authPath);

        int poemCount = 0;
        for (int i = 0; i < authLines.Length; i++)
        {
            if (authLines[i].Contains("full name = "))
            {
                Utilities.authName = authLines[i].Substring(12);
            }
            if (authLines[i].Contains("poem {")) 
            {
                poemCount++;
            }
        }

        int randomPoem = Random.Range(0, poemCount);
        Utilities.poemID = randomPoem;

        for (int i = 0; i < authLines.Length; i++)
        {
            if (authLines[i].Contains("id = " + randomPoem))
            {
                Utilities.poemTitle = authLines[i + 1].Substring(8);
            }
        }

        SceneManager.LoadScene(index);
    }
}

