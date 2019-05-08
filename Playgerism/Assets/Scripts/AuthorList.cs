using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class AuthorList : MonoBehaviour {

	// Use this for initialization
	void Start () {
        authIDs = new Dictionary<int, string>();
        poemIDs = new Dictionary<int, string>();

        isAuthorScreen = true;
        FillAuthorDictionary();
        DisplayItems();
    }


    // -- VARIABLES --
    Dictionary<int, string> authIDs;
    Dictionary<int, string> poemIDs;
    public GameObject listItemPrefab;
    public bool isAuthorScreen;

    private int currentAuth;
    private int currentPoem;


    // Update is called once per frame
    void Update () {
		
	}


    // EFFECTS: Fills the authIDs dictionary with all of the authors and their associated ids
    // MODIFIES: this
    // REQUIRES: nothing
    private void FillAuthorDictionary()
    {
        string[] data = GetAuthorData();

        foreach (string line in data)
        {
            string[] split = line.Split(',');
            int id = int.Parse(split[0]);
            authIDs.Add(id, split[1].Trim());
        }
    }


    // EFFECTS: reads the AuthIDs csv file and gets all the authors and their associated ids
    // MODIFIES: nothing
    // REQUIRES: nothing
    public static string[] GetAuthorData()
    {
        string[] authorData;
        string dir = Directory.GetCurrentDirectory();
        string path = dir + "\\Assets\\Resources\\Poems\\AuthIDs.csv";

        string[] temp = File.ReadAllLines(path);
        authorData = new string[temp.Length - 1];

        for (int i = 1; i < temp.Length; i++)
        {
            authorData[i - 1] = temp[i];
        }

        return authorData;
    }


    // TODO: display the itmes in alphebetical order
    // TODO: handle text overflows
    // TODO: have the back button go back to the authors if we're getting poems
    // EFFECTS: instantiates a list of authors or poems in the Unity scene
    // MODIFIES: the _CHOOSEPOEM_ scene
    // REQUIRES: nothing
    private void DisplayItems()
    {
        Dictionary<int, string> dict;
        int yPos = -15;
        int i = 0;

        if (isAuthorScreen)
        {
            dict = authIDs;
        }
        else
        {
            dict = poemIDs;
        }

        foreach (int key in dict.Keys)
        {
            Quaternion rotation = listItemPrefab.transform.rotation;
            Vector3 position = new Vector3(0, yPos, -1);

            GameObject instListItem = Instantiate(listItemPrefab, position, rotation, this.transform);
            instListItem.name = "Item " + i;
            instListItem.transform.position = new Vector3(0, yPos, -1);

            instListItem.transform.Find("Caption").GetComponent<TextMesh>().text = dict[key];

            ChooseElement item = instListItem.GetComponent<ChooseElement>();
            item.id = key;

            i++;
            yPos = yPos - 10;
        }
    }


    // EFFECTS: send the user to a new page displaying the poems under the selected author
    // MODIFIES: this, the "List Item" objects in the scene
    // REQUIRES: an author to have some poems in their text files?
    public void SelectAuthor(int authID)
    {
        currentAuth = authID;

        // so poems fill up per author
        poemIDs.Clear();

        // swap headers
        GetComponentInParent<Canvas>().transform.Find("Header").Find("Authors Title").gameObject.SetActive(false);
        GetComponentInParent<Canvas>().transform.Find("Header").Find("Poems Title").gameObject.SetActive(true);
        isAuthorScreen = false;

        DestroyAuthors();

        GetPoemData(authID);
        DisplayItems();
    }


    // EFFECTS: destroy all of the game objects that are children of the "List" object
    // MODIFIES: the "List" object's children
    // REQUIRES: nothing
    private void DestroyAuthors()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.Find("Item " + i).gameObject);
        }
    }


    // EFFECTS: reads the authID's text file and parses the poem's name and ids
    // MODIFIES: nothing
    // REQUIRES: nothing
    public void GetPoemData(int authID)
    {
        string[] poemData;
        string dir = Directory.GetCurrentDirectory();
        string path = dir + "\\Assets\\Resources\\Poems\\Authors\\" + authID + ".txt";

        int currentID = 0;
        string currentTitle = "";

        using (StreamReader sr = new StreamReader(path))
        {
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("id ="))
                {
                    currentID = int.Parse(line.Substring(5).Trim());
                }
                else if (line.Contains("title ="))
                {
                    currentTitle = line.Substring(8).Trim();
                    poemIDs.Add(currentID, currentTitle);
                }
            }
        }
    }



    // EFFECTS: End this scene, and call the new _SCENE_ with the selected poem
    // MODIFIES: this, the _SCENE_ scene, the _CHOOSEPOEM_ scene
    // REQUIRES: currentAuth to have been set
    public void SelectPoem(int poemID)
    {
        Utilities.SetIDs(currentAuth, poemID);
        SceneManager.LoadScene(0);
    }
}
