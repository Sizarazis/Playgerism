using System.Collections;
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
        Utilities.authID = Random.Range(0, lines.Length-2);
        string[] authLines;
        string path = "";

#if PLATFORM_ANDROID
        path = Application.streamingAssetsPath + "/Authors/" + Utilities.authID + ".txt";
        UnityWebRequest www = UnityWebRequest.Get(path);
        www.Send();
        while (!www.isDone)
        {
        }
        authLines = www.downloadHandler.text.Split('\n');
#elif PLATFORM_IOS
        path = Application.streamingAssetsPath + "/Authors/" + Utilities.authID + ".txt";
        authLines = File.ReadAllLines(path);
#else
        // WINDOWS EDITOR
        if (Utilities.GetOSVersion() == Utilities.OSVersion.Windows)
        {
            path = Application.streamingAssetsPath + "\\Authors\\" + Utilities.authID + ".txt";
        }
        // OSX EDITOR 
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

