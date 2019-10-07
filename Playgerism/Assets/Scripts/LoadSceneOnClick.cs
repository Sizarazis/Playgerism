﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Networking;

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

        string tp = Resources.Load<TextAsset>("AuthIDs").ToString();
        string[] lines = tp.Split('\n');

        int randomAuth = UnityEngine.Random.Range(1, lines.Length);
        Utilities.authID = randomAuth - 1;
        string[] authLines;

#if PLATFORM_ANDROID
        var _path = Application.streamingAssetsPath + "/Authors/" + Utilities.authID + ".txt";
        UnityWebRequest www = UnityWebRequest.Get(_path);
        www.Send();
        while (!www.isDone)
        {
        }
        authLines = www.downloadHandler.text.Split('\n');
#else
        string path = Application.streamingAssetsPath + "\\Authors\\" + Utilities.authID + ".txt";

        if (Utilities.GetOSVersion() == Utilities.OSVersion.MacOSX)
        {
            path = Application.streamingAssetsPath + "/Authors/" + Utilities.authID + ".txt";
        }
        authLines = File.ReadAllLines(path);
#endif

        int poemCount = 0;
        for (int i = 0; i < authLines.Length; i++)
        {
            if (authLines[i].Contains("full name = "))
            {
                Utilities.authName = authLines[i].Substring(12).Trim();
            }
            if (authLines[i].Contains("poem {")) 
            {
                poemCount++;
            }
        }

        int randomPoem = UnityEngine.Random.Range(0, poemCount);
        Utilities.poemID = randomPoem;

        for (int j = 0; j < authLines.Length; j++)
        {
            if (authLines[j].Contains("id = " + randomPoem))
            {
                Utilities.poemTitle = authLines[j + 1].Substring(8).Trim();
            }
        }

        SceneManager.LoadScene(index);
    }
}

