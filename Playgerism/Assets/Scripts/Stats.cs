using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Stats : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetStats();
        DisplayStats();
	}


    // -- VARIABLES --
    public GameObject statRecordPrefab;
    private string[,] stats;


	// Update is called once per frame
	void Update () {
		
	}


    // TODO: Test
    // EFFECTS: Sets the height of the Viewport to enable the whole list to be scrolled
    // MODIFIES: this
    // REQUIRES: nothing
    void SetContentHeight(int numItems)
    {
        RectTransform content = transform.GetComponentInParent<RectTransform>();
        content.offsetMin = new Vector2(content.offsetMin.x, -numItems);
        content.offsetMax = new Vector2(content.offsetMax.x, 0);

    }


    // EFFECTS: Gets the poem titles and author names and thWeir associated best times from resources
    // MODIFIES: this
    // REQUIRES: nothing
    private void GetStats()
    {
        string dir = Directory.GetCurrentDirectory();
        string path = dir + "\\Assets\\Resources\\UserStats.csv";
        string[] lines = File.ReadAllLines(path);

        if (lines.Length < 1) return;
        else
        {
            stats = new string[lines.Length - 1, 3];

            for (int i = 1; i < lines.Length; i++)
            {
                string[] split = lines[i].Split(',');
                stats[i - 1, 0] = split[0];             // author name
                stats[i - 1, 1] = split[1];             // poem name
                stats[i - 1, 2] = split[2];             // record time
            }
        }
    }


    // EFFECTS: Displays the stats
    // MODIFIES: this
    // REQUIRES: nothing
    private void DisplayStats()
    {
        Vector3 position = statRecordPrefab.transform.localPosition;
        Quaternion rotation = statRecordPrefab.transform.localRotation;

        for (int i = 0; i < stats.GetLength(0); i++)
        {
            string author = stats[i, 0].Trim();
            string poem = stats[i, 1].Trim();
            string time = stats[i, 2].Trim();

            position = new Vector3(0, -28 - (i * 10), -1);
            GameObject stat = Instantiate(statRecordPrefab, position, rotation, this.transform);

            stat.transform.Find("Author").GetComponent<Text>().text = author;
            stat.transform.Find("Title").GetComponent<Text>().text = poem;
            stat.transform.Find("Time").GetComponent<TextMesh>().text = time;
        }
    }
}
