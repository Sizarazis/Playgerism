using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.Advertisements;


public class StateManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // SET TIMER
        timer = 0.0f;
        minutes = 0;

        // SET END AND MENU VARS
        isPaused = false;
        isEnd = false;
        endHandled = false;

        // SET PLATFORM SPECIFIC VARS
        canvasWidth = GetComponent<RectTransform>().sizeDelta.x;
        OSVersion = Utilities.GetOSVersion();

        // BUILD THE POEM
        transform.Find("Scroll View/Viewport/Content/Title/Title").GetComponent<TextMesh>().text = Utilities.poemTitle;
        transform.Find("Scroll View/Viewport/Content/Title/Author").GetComponent<TextMesh>().text = Utilities.authName;
        poem = Utilities.ParsePoem();
        scrambledPoem = ScramblePoem();
        solution = new HashSet<Line>();

        // BUILD THE BOARD
        SetTitleHeaderWidth();
        BuildSlots();
        BuildLines();
        ConnectSlots();


        // CONNECT THE POEM TO THE BOARD
        AttachLinesAndSlots();
        SetContentHeight();

        Advertisement.Banner.Hide();
    }


    // -- VARIABLES -- //
    public GameObject slotPrefab;
    public GameObject slots;
    public GameObject linePrefab;
    public GameObject lines;
    public GameObject menu;

    private string[] poem;
    private string[] scrambledPoem;
    private HashSet<Line> solution;
    private int lineHeight = 60;
    private int minutes;
    private float timer;
    private float canvasWidth;
    private bool isPaused;
    private bool isEnd;
    private bool endHandled;

    private Utilities.OSVersion OSVersion;

    // Update is called once per frame
    void Update () {
        CheckPause();

        if (!isEnd && !isPaused)
        {
            CheckLineMatches();

            if (minutes < 100)
            {
                UpdateTimer();
            }

            CheckEnd();
        }
        else
        {
            if (!endHandled && isEnd)
            {
                HandleEnd();
                endHandled = true;
            }
        }
    }


    void SetTitleHeaderWidth()
    {
        Transform titleHeader = transform.Find("Scroll View/Viewport/Content/Title/Background");

        titleHeader.localScale = new Vector3(canvasWidth/10, 1, 8);
        titleHeader.localPosition = new Vector3(canvasWidth/2, -40, 0);
    }


    // EFFECTS: Sets the height of the Content window to enable the whole poem to be scrolled
    // MODIFIES: this
    // REQUIRES: nothing
    void SetContentHeight()
    {
        int headerSize = 160;

        Transform content = transform.Find("Scroll View").Find("Viewport").Find("Content");
        RectTransform rectTransform = content.GetComponent<RectTransform>();

        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, -lineHeight*poem.Length + (GetComponent<RectTransform>().sizeDelta.y - headerSize));
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 0);
    }


    // EFFECTS: randomly shuffles the lines of the poem after the first line
    // MODIFIES: nothing
    // REQUIRES: nothing
    string[] ScramblePoem()
    {
        int len = poem.Length;
        string[] res = new string[len];

        for (int i = 0; i < len; i++)
        {
            res[i] = poem[i];
        }

        for (int j = 1; j < len; j++)
        {
            string temp = res[j];
            int random = UnityEngine.Random.Range(j, len);
            res[j] = res[random];
            res[random] = temp;
        }
        return res;
    }


    // EFFECTS: instantiates slots from the slot prefab
    // MODIFIES: the slots gameObject
    // REQUIRES: slotPrefab and slots to reference a prefab and gameObject in the Unity project
    void BuildSlots()
    {
        int yPos = -110;

        for (int i = 0; i < poem.Length; i++)
        {
            Quaternion rotation = slotPrefab.transform.rotation;
            Vector3 position = new Vector3(canvasWidth/2, yPos, 1);

            GameObject instSlot = Instantiate(slotPrefab, position, rotation, slots.transform);
            instSlot.transform.localPosition = position;
            instSlot.transform.localScale = new Vector3(canvasWidth/10, 1, lineHeight/10);

            instSlot.name = "Slot " + i;
            instSlot.GetComponent<Slot>().position = i;

            yPos = yPos - lineHeight;
        }
    }


    // EFFECTS: instantiates links and lines from their prefabs
    // MODIFIES: the lines and links gameObjects
    // REQUIRES: references prefabs and gameObjects in the Unity project
    void BuildLines()
    {
       int yPos = -110;

        for (int i = 0; i < poem.Length; i++)
        {
            Quaternion rotation = linePrefab.transform.rotation;
            Vector3 position = new Vector3(canvasWidth/2, yPos, -2);
            GameObject line = Instantiate(linePrefab, position, rotation, lines.transform);
            line.transform.localPosition = new Vector3(canvasWidth/2, yPos, -2);
            line.transform.localScale = new Vector3(canvasWidth/100, lineHeight/10, 1);

            line.name = "Line " + i;
            line.transform.Find("Text").GetComponent<TextMesh>().text = scrambledPoem[i];

            Line line_script = line.GetComponent<Line>();
            line_script.text = scrambledPoem[i];

            //Get for line copies
            int occurances = 1;
            for (int j=0; j<i; j++)
            {
                if (scrambledPoem[j] == scrambledPoem[i])
                {
                    occurances++;
                }
            }

            //Set id's according to how many times this line is repeated in the poem
            for (int k=0; k<poem.Length; k++)
            {
                if (scrambledPoem[i] == poem[k])
                {
                    if (occurances == 1)
                    {
                        line_script.id = k;
                        break;
                    }
                    else
                    {
                        occurances--;
                    }
                }
            }

            yPos = yPos - lineHeight;
        }
    }


    // EFFECTS: sets the previous and next slots for each given slot
    // MODIFIES: the Slot gameObjects
    // REQUIRES: nothing
    void ConnectSlots()
    {
        Slot prev = null;
        foreach (Slot slot in slots.transform.GetComponentsInChildren<Slot>())
        {
            if (prev != null)
            {
                slot.prev = prev;
                prev.next = slot;
            }
            prev = slot;
        }
    }


    // EFFECTS: initially attaches the Lines to Slots, and Slots to Lines
    // MODIFIES: the Line and Slot gameObjects
    // REQUIRES: nothing
    void AttachLinesAndSlots()
    {
        Slot[] allSlots = slots.transform.GetComponentsInChildren<Slot>();
        Line[] allLines = lines.transform.GetComponentsInChildren<Line>();

        for (int i = 0; i<allLines.Length; i++)
        {
            allLines[i].slot = allSlots[i];
            allSlots[i].line = allLines[i];
        }
    }


    // EFFECTS: calls UpdateMatches() on every line in the poem. Ensures that each line's matches are up-to-date
    // MODIFIES: every line object in the poem
    // REQUIRES: nothing
    void CheckLineMatches()
    {
        foreach (Line line in lines.transform.GetComponentsInChildren<Line>())
        {
            line.UpdateMatches();

            if (line.solved && !solution.Contains(line))
            {
                solution.Add(line);
                UpdateLineColors(line);
            }
        }
    }


    // NOTE: Think about adding a list of acceptable colours and giving each link one of them
    // EFFECTS: modifies the color of all lines in the poem according to how many matches they have
    // MODIFIES: the line object's background color
    // REQUIRES: the background to have a MeshRenderer
    void UpdateLineColors(Line line)
    {
        Color initial = new Color (0.94117f, 0.94117f, 0.94117f, 1);
        Color green = new Color(118 / 255f, 219 / 255f, 148 / 255f, 1);
        Color color;

        if (line.solved)
        {
            color = green;
        }
        else color = initial;

        line.gameObject.transform.Find("Background").GetComponent<MeshRenderer>().material.color = color;
    }


    // EFFECTS: updates the game timer
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

        GameObject.Find("Timer Text").GetComponent<TextMesh>().text = "Timer: " + outMin + ":" + outSec;   
    }


    // EFFECTS: checks if the game has ended
    // MODIFIES: isEnd
    // REQUIRES: nothing
    private void CheckEnd()
    {
        foreach (Line line in lines.GetComponentsInChildren<Line>())
        {
            if (solution.Count != poem.Length)
            {
                isEnd = false;
                return;
            }
        }

        isEnd = true;
    }


    // EFFECTS: checks if the game is paused
    // MODIFIES: isPaused
    // REQUIRES: the menu gameObject
    private void CheckPause()
    {
        if (!menu.activeInHierarchy && !isEnd)
        {
            isPaused = false;
            transform.Find("Header/Pause Button").GetComponent<Button>().interactable = true;
        }
        else if (menu.activeInHierarchy)
        {
            isPaused = true;
            transform.Find("Header/Pause Button").GetComponent<Button>().interactable = false;
        }
    }


    // EFFECTS: Go through the end game procedures
    // MODIFIES: this/end popup
    // REQUIRES: nothing
    private void HandleEnd()
    {
        BoxCollider2D[] lineColliders = lines.transform.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D col in lineColliders)
        {
            col.enabled = false;
        }

        GetEndStats();

        transform.Find("Header/Pause Button").GetComponent<Button>().interactable = false;
        transform.Find("Scroll View/End Popup").gameObject.SetActive(true);
    }

    // EFFECTS: Get the stats for the game's end
    // MODIFIES: this/end popup
    // REQUIRES: nothing
    private void GetEndStats()
    {
        TextMesh toModify = transform.Find("Scroll View/End Popup/End Panel/Results").gameObject.GetComponent<TextMesh>();

        string prevBest;
        string time;

        prevBest = ParseBestTime();

        time = transform.Find("Header/Timer Text").GetComponent<TextMesh>().text.Substring(7);

        toModify.text = prevBest + "\n" + time;
    }


    // EFFECTS: parses the userStats.csv for their record poem time. If they don't have a record, then it will insert this one. If they're new time is better, it will update the userStats
    // MODIFIES: this, userStats.csv
    // REQUIRES: nothing
    private string ParseBestTime()
    {
        string time = "";
        string path = Application.persistentDataPath + "/userStats.csv";

        if (Utilities.GetOSVersion() == Utilities.OSVersion.Windows)
        {
            path = Application.persistentDataPath + "\\userStats.csv";
        }

        bool foundRecord = false;
        bool newBest = false;

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
            InsertNewRecord();

            return transform.Find("Header/Timer Text").GetComponent<TextMesh>().text.Substring(7);
        }


        using (var reader = new StreamReader(path))
        {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains(Utilities.authName + ", " + Utilities.poemTitle))
                    {
                        foundRecord = true;

                        string[] temp;
                        temp = line.Split(',');

                        time = temp[temp.Length-1].Trim();

                        newBest = CompareTimes(time);

                        if (newBest)
                        {
                            string record;
                            string newTime = transform.Find("Header/Timer Text").GetComponent<TextMesh>().text.Substring(7);

                            record = Utilities.authName + ", " + Utilities.poemTitle + ", " + newTime;
                        }

                        break;
                    }
                }
        }

        if (!foundRecord)
        {
            InsertNewRecord();
        }

        if (foundRecord && newBest)
        {
            UpdateRecordTime();
        }

        return time;
    }


    // EFFECTS: compares the previous best time with the new time
    // MODIFIES: nothing
    // REQUIRES: nothing
    private bool CompareTimes(string prevTime)
    {
        bool newBest = false;
        int newHours = 0;
        int newMins = 0;

        int prevHours = 0;
        int prevMins = 0;

        // get new times
        string newTime = transform.Find("Header/Timer Text").GetComponent<TextMesh>().text.Substring(7);
        string[] newTimeSplit = newTime.Split(':');
        newHours = int.Parse(newTimeSplit[0].Trim());
        newMins = int.Parse(newTimeSplit[1].Trim());


        // get old times
        string[] prevTimeSplit = prevTime.Split(':');
        prevHours = int.Parse(prevTimeSplit[0].Trim());
        prevMins = int.Parse(prevTimeSplit[1].Trim());

        if (newHours < prevHours)
        {
            newBest = true;
        }

        if (newHours == prevHours && newMins < prevMins)
        {
            newBest = true;
        }

        return newBest;
    }


    // EFFECTS: insert an entirely new record for the poem
    // MODIFIES: UserStats.csv
    // REQUIRES: nothing
    private void InsertNewRecord()
    {
        string path = Application.persistentDataPath + "/userStats.csv";

        if (Utilities.GetOSVersion() == Utilities.OSVersion.Windows)
        {
            path = Application.persistentDataPath + "\\userStats.csv";
        }

        string record;
        string newTime = transform.Find("Header/Timer Text").GetComponent<TextMesh>().text.Substring(7);

        record = Utilities.authName + ", " + Utilities.poemTitle + ", " + newTime;

        using (StreamWriter w = File.AppendText(path))
        {
            w.WriteLine(record);
        }

    }


    // EFFECTS: update the record of this poem with a new best time
    // MODIFIES: UserStats.csv
    // REQUIRES: nothing
    private void UpdateRecordTime()
    {
        string path = Application.persistentDataPath + "/userStats.csv";

        if (Utilities.GetOSVersion() == Utilities.OSVersion.Windows)
        {
            path = Application.persistentDataPath + "\\userStats.csv";
        }

        string[] statLines = File.ReadAllLines(path);
        string newRecord;
        string newTime = transform.Find("Header/Timer Text").GetComponent<TextMesh>().text.Substring(7);
        newRecord = Utilities.authName + ", " + Utilities.poemTitle + ", " + newTime;
        newRecord.Trim();

        for(int i = 0; i < statLines.Length; i++)
        { 
            if (statLines[i].Contains(Utilities.authName + ", " + Utilities.poemTitle))
            {
                statLines[i] = newRecord;
            }
        }

        File.WriteAllLines(path, statLines);
    }
}
