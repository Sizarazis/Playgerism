using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StateManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        timer = 0.0f;
        minutes = 0;
        isPaused = false;
        isEnd = false;
        endHandled = false;

        poem = BuildPoem();
        numLines = poem.Length;
        numSlots = poem.Length;
        scrambledPoem = ScramblePoem();

        BuildSlots();
        BuildLinksAndLines();
        SetLineText();
        ConnectSlots();

        lines = GetLines();
        AttachLinesAndSlots();
        AttachLinesAndLinks();
        SetContentHeight();
    }


    // -- VARIABLES -- //
    public struct IDLine
    {
        public int ID;
        public string line;
        public bool isLast;
    }
    public GameObject slotPrefab;
    public GameObject slots;
    public GameObject linePrefab;
    public GameObject linesObj;
    public GameObject lineLinks;
    public GameObject linkPrefab;
    public GameObject menu;

    private Line[]      lines;
    private IDLine[]    poem;
    private IDLine[]    scrambledPoem;
    private int         numLines;
    private int         numSlots;
    private int         lineHeight = 10;
    private int         minutes;
    private float       timer;
    private bool        isPaused;
    private bool        isEnd;
    private bool        endHandled;

    // Update is called once per frame
    void Update () {
        CheckPause();

        if (!isEnd && !isPaused)
        {
            CheckLineMatches();
            UpdateLineLinks();

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


    // EFFECTS: Sets the height of the Viewport to enable the whole poem to be scrolled
    // MODIFIES: this
    // REQUIRES: nothing
    void SetContentHeight()
    {
        Transform content = transform.Find("Scroll View").Find("Viewport").Find("Content");
        RectTransform rectTransform = content.GetComponent<RectTransform>();
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, -10*numSlots + 126);
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 0);
    }


    // EFFECTS: builds an array of lines with IDs attached
    // MODIFIES: this
    // REQUIRES: stringPoems and numLines to be instantiated
    IDLine[] BuildPoem()
    {
        string[] lines = Utilities.ParsePoem();
        int size = lines.Length;

        transform.Find("Scroll View").Find("Viewport").Find("Content").Find("Poem").Find("Title").Find("Title").GetComponent<TextMesh>().text = Utilities.poemTitle;
        transform.Find("Scroll View").Find("Viewport").Find("Content").Find("Poem").Find("Title").Find("Author").GetComponent<TextMesh>().text = Utilities.authName;

        IDLine[] output = new IDLine[size];
        for (int i = 0; i < size; i++)
        {
            output[i].line = lines[i];
            output[i].ID = i;
            output[i].isLast = false;
            if (i == size - 1)
            {
                output[i].isLast = true;
            }
        }

        return output;
    }


    // EFFECTS: randomly shuffles the poem's lines
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


    Line[] GetLines()
    {
        return lineLinks.GetComponentsInChildren<Line>();
    }


    Link[] GetLinks()
    {
        return lineLinks.GetComponentsInChildren<Link>();
    }


    // EFFECTS: instantiates slots from the slot prefab
    // MODIFIES: the slots gameObject
    // REQUIRES: slotPrefab and slots to reference a prefab and gameObject in the Unity project
    void BuildSlots()
    {
        int yPos = -25;

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


    // EFFECTS: instantiates links and lines from their prefabs
    // MODIFIES: the lines and links gameObjects
    // REQUIRES: references prefabs and gameObjects in the Unity project
    void BuildLinksAndLines()
    {
        int yPos = -25;

        for (int i = 0; i < numLines; i++)
        {
            Quaternion linkRot = linkPrefab.transform.rotation;
            Vector3 linkPos = new Vector3(0, yPos, -2);
            GameObject instLink = Instantiate(linkPrefab, linkPos, linkRot, lineLinks.transform);
            instLink.name = "Link " + i;

            Quaternion lineRot = linePrefab.transform.rotation;
            Vector3 linePos = new Vector3(0, yPos, 0);
            GameObject instLine = Instantiate(linePrefab, linePos, lineRot, instLink.transform);
            instLine.name = "Line " + i;

            yPos = yPos - lineHeight;
        }
    }


    // EFFECTS: sets the lines 
    // MODIFIES: the text displayed in each Lines/Line/Text gameObject
    // REQUIRES: lines to have already instantiated its line children
    void SetLineText()
    {
        //Transform lineLoc = lineLinks.Get
        int i = 0;
        foreach(Transform line in lineLinks.GetComponentsInChildren<Transform>())
        {
            if (line.name == "Text")
            {
                line.GetComponentInChildren<TextMesh>().text = scrambledPoem[i].line;
                line.GetComponentInParent<Line>().lineID = scrambledPoem[i].ID;
                line.GetComponentInParent<Line>().isLast = scrambledPoem[i].isLast;

                i++;
            }
        }
    }


    // EFFECTS: sets the previous and next slots for each given slot
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


    // EFFECTS: initially attaches the Lines to Slots, and Slots to Lines
    // MODIFIES: the Line and Slot gameObjects
    // REQUIRES: nothing
    void AttachLinesAndSlots()
    {
        Slot[] allSlots = slots.transform.GetComponentsInChildren<Slot>();

        int i = 0;
        foreach (Line instLine in lineLinks.transform.GetComponentsInChildren<Line>())
        {
            instLine.inSlot = allSlots[i];
            allSlots[i].currentLine = instLine;

            i++;
        }
    }


    // EFFECTS: initially attaches the Lines to Links and Links to Lines
    // MODIFIES: the Link game objects
    // REQUIRES: the lines[] array to be instantiated first
    void AttachLinesAndLinks()
    {
        int i = 0;
        foreach (Link link in lineLinks.transform.GetComponentsInChildren<Link>())
        {
            link.lines.Add(lines[i]);
            lines[i].inLink = link;
            i++;
        }
    }


    // EFFECTS: calls UpdateMatches() on every line in the poem. Ensures that each line's matches are up-to-date
    // MODIFIES: every line object in the poem
    // REQUIRES: nothing
    void CheckLineMatches()
    {
        lines = GetLines();
        foreach(Line line in lines)
        {
            line.UpdateMatches();
        }
    }


    // EFFECTS: moves a line to its correct link
    // MODIFIES: line and link objects
    // REQUIRES: nothing
    void UpdateLineLinks()
    {
        lines = GetLines();
        if (lines.Length <= 0) return;

        foreach (Line line in lines)
        {
            if (line.topMatch || line.bottomMatch)
            {
                if (line.topMatch && line.lineID > 0)
                {
                    line.inSlot.prevSlot.currentLine.inLink.MergeLinks(line.inLink);
                }
                if (line.bottomMatch && line.isLast == false)
                {
                    line.inLink.MergeLinks(line.inSlot.nextSlot.currentLine.inLink);
                }
            }
            UpdateLineColors(line);
        }
    }


    // NOTE: Think about adding a list of acceptable colours and giving each link one of them
    // EFFECTS: modifies the color of all lines in the poem according to how many matches they have
    // MODIFIES: the line object's background color
    // REQUIRES: the background to have a MeshRenderer
    void UpdateLineColors(Line line)
    {
        Color baseColor = new Color (0.94117f, 0.94117f, 0.94117f, 1);
        Color green = new Color(0.23529f, 0.68627f, 0.39215f, 1);
        Color test = new Color(118/255f, 219/255f, 148/255f, 1);
        Color outColor;

        if (line.topMatch || line.bottomMatch)
        {
            outColor = test;
        }
        else outColor = baseColor;

        line.gameObject.transform.Find("Background").GetComponent<MeshRenderer>().material.color = outColor;
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

        //NOTE: THIS GAMEOBJECT WILL CHANGE NAMES
        gameObject.transform.Find("Timer Text").GetComponent<TextMesh>().text = "Timer: " + outMin + ":" + outSec;   
    }


    // EFFECTS: checks if the game has ended
    // MODIFIES: isEnd
    // REQUIRES: nothing
    private void CheckEnd()
    {
        foreach (Line line in lines)
        {
            if (line.lineID != line.inSlot.relPosition)
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
        if (!menu.activeInHierarchy)
        {
            isPaused = false;
        }
        else
        {
            isPaused = true;
        }
    }


    // EFFECTS: Go through the end game procedures
    // MODIFIES: this/end popup
    // REQUIRES: nothing
    private void HandleEnd()
    {
        GetEndStats();
        transform.Find("Scroll View").Find("End Popup").gameObject.SetActive(true);
        //transform.Find("Scroll View").gameObject.SetActive(false);
        transform.Find("Scroll View").Find("Viewport").Find("Content").Find("Poem").gameObject.transform.Find("Links").transform.GetChild(0).transform.GetComponent<BoxCollider2D>().enabled = false;
    }


    // EFFECTS: Get the stats for the game's end
    // MODIFIES: this/end popup
    // REQUIRES: nothing
    private void GetEndStats()
    {
        TextMesh toModify = transform.Find("Scroll View").Find("End Popup").gameObject.transform.Find("Results").gameObject.GetComponent<TextMesh>();

        string prevBest;
        string time;

        prevBest = ParseBestTime();

        time = transform.Find("Timer Text").GetComponent<TextMesh>().text.Substring(7);

        toModify.text = prevBest + "\n" + time;
    }


    // EFFECTS: parses the userStats.csv for their record poem time. If they don't have a record, then it will insert this one. If they're new time is better, it will update the userStats
    // MODIFIES: this, userStats.csv
    // REQUIRES: nothing
    private string ParseBestTime()
    {
        string time = "";

        string dir = Directory.GetCurrentDirectory();
        string path = dir + "\\Assets\\Resources\\UserStats.csv";

        bool foundRecord = false;
        bool newBest = false;

        using (var reader = new StreamReader(path))
        {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains(Utilities.authName + ", " + Utilities.poemTitle))
                    {
                        foundRecord = true;

                        string[] temp = new string[3];
                        temp = line.Split(',');

                        time = temp[2].Trim();

                        newBest = CompareTimes(time);

                        if (newBest)
                        {
                            string record;
                            string newTime = transform.Find("Timer Text").GetComponent<TextMesh>().text.Substring(7);

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
        string newTime = transform.Find("Timer Text").GetComponent<TextMesh>().text.Substring(7);
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
        string dir = Directory.GetCurrentDirectory();
        string path = dir + "\\Assets\\Resources\\UserStats.csv";

        string record;
        string newTime = transform.Find("Timer Text").GetComponent<TextMesh>().text.Substring(7);

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
        string dir = Directory.GetCurrentDirectory();
        string path = dir + "\\Assets\\Resources\\UserStats.csv";
        string[] lines = File.ReadAllLines(path);

        string newRecord;
        string newTime = transform.Find("Timer Text").GetComponent<TextMesh>().text.Substring(7);
        newRecord = Utilities.authName + ", " + Utilities.poemTitle + ", " + newTime;
        newRecord.Trim();

        for(int i = 0; i < lines.Length; i++)
        { 
            if (lines[i].Contains(Utilities.authName + ", " + Utilities.poemTitle))
            {
                lines[i] = newRecord;
            }
        }

        File.WriteAllLines(path, lines);
    }
}