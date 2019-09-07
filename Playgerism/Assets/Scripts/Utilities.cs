using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
        string dir = Directory.GetCurrentDirectory();
        string path = dir + "\\Assets\\Resources\\Poems\\Authors\\" + authID + ".txt";

        if (GetOSVersion() == OSVersion.MacOSX)
        {
            path = dir + "//Assets//Resources//Poems//Authors//" + authID + ".txt";
        }

        using (var reader = new StreamReader(path))
        {
            bool inPoem = false;
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

                    inPoem = true;
                }
                if (inPoem)
                {
                    for (int i = 0; i < poemSize; i++)
                    {
                        poem[i] = reader.ReadLine().Trim();
                    }
                    inPoem = false;
                    break;
                }
            }
        }

        return poem;
    }
}
