using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System;

public static class Utilities {
    public static int authID;
    public static int poemID;
    public static string authName;
    public static string poemTitle;

    public enum OSVersion
    {
        Windows,
        MacOSX,
        Invalid
    }

    // EFFECTS: updates the authID and poemID to the currently selected poem
    // MODIFIES: this
    // REQUIRES: nothing
    public static void SetIDs(int currentAuth, int currentPoem)
    {
        authID = currentAuth;
        poemID = currentPoem;
    }

    // EFFECTS: Checks the OS Version and sets the deliminator between paths
    // MODIFIES: nothing
    // REQUIRES: nothing
    public static OSVersion GetOSVersion()
    {
        System.OperatingSystem os = System.Environment.OSVersion;
        System.PlatformID pid = os.Platform;

        switch (pid)
        {
            case System.PlatformID.Win32NT:
            case System.PlatformID.Win32S:
            case System.PlatformID.Win32Windows:
            case System.PlatformID.WinCE:
                return OSVersion.Windows;
            case System.PlatformID.Unix:
            case System.PlatformID.MacOSX:
                return OSVersion.MacOSX;
            default:
                return OSVersion.Invalid;
        }
    }


    // EFFECTS: Parses a poem from an author's text file
    // MODIFIES: nothing
    // REQUIRES: authID and poemID to be set
    public static string[] ParsePoem()
    {
        string[] poem = new string[0];
        int poemSize = 0;

//JUST FOR TESTING
#if UNITY_EDITOR
        string dir = Application.streamingAssetsPath;
        string path = dir + "\\Authors\\" + authID + ".txt";

        if (GetOSVersion() == OSVersion.MacOSX)
        {
            path = dir + "//Authors//" + authID + ".txt";
        }

        using (var reader = new StreamReader(path))
        {
            bool inPoemEditor = false;
            while (!reader.EndOfStream)
            {
                if (reader.ReadLine().Contains("id = " + poemID))   // id =
                {
                    string[] split = new string[2];

                    reader.ReadLine();                              // title =
                    split = reader.ReadLine().Split('=');           // size =
                    split[1].Trim();
                    poemSize = int.Parse(split[1]);

                    poem = new string[poemSize];

                    reader.ReadLine();                              // lines = {

                    inPoemEditor = true;
                }
                if (inPoemEditor)
                {
                    for (int k = 0; k < poemSize; k++)
                    {
                        poem[k] = reader.ReadLine().Trim();
                    }
                    inPoemEditor = false;
                    break;
                }
            }
        }
#endif

#if PLATFORM_ANDROID
        var _path = Application.streamingAssetsPath + "/Authors/" + authID + ".txt";

        UnityWebRequest www = UnityWebRequest.Get(_path);
        www.Send();
        while (!www.isDone)
        {
        }
        //Debug.Log(www.downloadHandler.text);
        string[] content = www.downloadHandler.text.Split('\n');

        bool inPoem = false;
        bool poemStarted = false;
        for (int i=0; i<content.Length; i++)
        {

            if (content[i].Contains("id = " + poemID))   // id =
            {
                poemStarted = true;
                continue;
            }
            if (content[i].Contains("title = "))
            {
                continue;
            }
            if (content[i].Contains("size = ") && poemStarted)
            {
                string[] split = content[i].Split('=');
                split[1].Trim();
                poemSize = int.Parse(split[1]);
                poem = new string[poemSize];
            }
            if (content[i].Contains("lines = ") && poemStarted) {
                inPoem = true;
                continue;
            }
            if (inPoem && poemStarted)
            {
                for (int j = 0; j < poemSize; j++)
                {
                    poem[j] = content[i + j].Trim();
                }
                inPoem = false;
                poemStarted = false;
            }
        }
#endif
        return poem;
    }

    // EFFECTS: Gets the poem titles and author names and their associated best times from resources
    // MODIFIES: this
    // REQUIRES: nothing
    public static string[,] GetStats()
    {
        string[] lines;
        string path = Application.persistentDataPath + "/userStats.csv";

        if (!File.Exists(path)) return null;

        lines = File.ReadAllLines(path);
        Array.Sort(lines);

        if (lines.Length < 1) return null;
        string[,] stats = new string[lines.Length, 3];

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Equals("authorName,poemTitle,bestTime"))
            {
                continue;
            }
            if (lines[i] == null || !lines[i].Contains(","))
            {
                continue;
            }

            string[] split = lines[i].Split(',');

            stats[i, 0] = split[0]; // author name

            stats[i, 1] = split[1]; // poem name
            for (int j = 2; j < split.Length - 1; j++)
            {
                stats[i, 1] = stats[i, 1] + "," + split[j]; // for when poems have commas
            }
            stats[i, 2] = split[split.Length - 1]; // record time
        }
        return stats;
    }
}
