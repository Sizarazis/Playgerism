using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class Stats : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetStats();
        DisplayStats();
	}


    // -- VARIABLES --
    public GameObject statRecordPrefab;
    private string[,] stats;
    private float statSize = 12;


	// Update is called once per frame
	void Update () {

	}


    // TODO: Test
    // EFFECTS: Sets the height of the Viewport to enable the whole list to be scrolled
    // MODIFIES: this
    // REQUIRES: nothing
    void SetContentHeight(int numItems)
    {
        RectTransform content = transform.GetComponent<RectTransform>();
        content.offsetMin = new Vector2(content.offsetMin.x, -3 * statSize * numItems + 335);
        content.offsetMax = new Vector2(content.offsetMax.x, 0);
    }


    // EFFECTS: Gets the poem titles and author names and thWeir associated best times from resources
    // MODIFIES: this
    // REQUIRES: nothing
    private void GetStats()
    {
        string[] lines;
        string path = Application.persistentDataPath + "/userStats.csv";

        if (!File.Exists(path)) return;

        lines = File.ReadAllLines(path);
        Array.Sort(lines);

        if (lines.Length < 1) return;

        stats = new string[lines.Length - 1, 3];

        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i] == null || !lines[i].Contains(",")) break;

            string[] split = lines[i].Split(',');

            stats[i - 1, 0] = split[0]; // author name

            stats[i - 1, 1] = split[1]; // poem name
            for (int j=2; j<split.Length-1; j++)
            {
                stats[i - 1, 1] = stats[i - 1, 1] + "," + split[j]; // for when poems have commas
            }
            stats[i - 1, 2] = split[split.Length-1]; // record time
        }
    }


    // EFFECTS: Displays the stats
    // MODIFIES: this
    // REQUIRES: nothing
    private void DisplayStats()
    {
        while (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0));
        }

        Vector3 position = statRecordPrefab.GetComponent<RectTransform>().localPosition;
        Quaternion rotation = statRecordPrefab.GetComponent<RectTransform>().localRotation;

        if (stats == null) return;

        SetContentHeight(stats.GetLength(0));

        for (int i = 0; i < stats.GetLength(0); i++)
        {
            if (stats[i, 0] == null) continue;

            string author = stats[i, 0].Trim();
            string poem = stats[i, 1].Trim();
            string time = stats[i, 2].Trim();

            //position = new Vector3(0, (-i*statSize)/(float)1.26, 0);
            position = new Vector3(0, (-i * statSize)-5, 0);
            GameObject stat = Instantiate(statRecordPrefab, position, rotation, this.transform);

            //stat.transform.localScale = new Vector3(xScaler, stat.transform.localScale.y, stat.transform.localScale.z);
            stat.transform.Find("Author").GetComponent<TextMesh>().text = author;
            stat.transform.Find("Title").GetComponent<TextMesh>().text = poem;
            stat.transform.Find("Time").GetComponent<TextMesh>().text = time;
        }
    }
}
