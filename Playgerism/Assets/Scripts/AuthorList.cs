using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.Networking;

public class AuthorList : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DestroyItems();

        canvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x;
        authIDs = new Dictionary<int, string>();
        poemIDs = new Dictionary<int, string>();
        stats = Utilities.GetStats();

        isAuthorScreen = true; 
        FillAuthorDictionary();
        DisplayItems();
        Advertisement.Banner.Hide();
    }


    // -- VARIABLES --
    Dictionary<int, string> authIDs;
    Dictionary<int, string> poemIDs;
    public GameObject listItemPrefab;
    public bool isAuthorScreen;

    private float canvasWidth;
    private int currentAuth;
    private int currentPoem;
    private int offset = 16;
    private string[,] stats;


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
            if (line.Contains(","))
            {
                string[] split = line.Split(',');
                int id = int.Parse(split[0]);
                authIDs.Add(id, split[1].Trim());
            }
        }
    }


    // EFFECTS: reads the AuthIDs csv file and gets all the authors and their associated ids
    // MODIFIES: nothing
    // REQUIRES: nothing
    public static string[] GetAuthorData()
    {
        string[] authorData;
        string tp = Resources.Load<TextAsset>("AuthIDs").ToString();
        string[] lines = tp.Split('\n');

        authorData = new string[lines.Length - 1];

        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i] == null) break;
            authorData[i - 1] = lines[i];
        }

        return authorData;
    }


    // TODO: handle horizontal text overflows
    // TODO: have the back button go back to the authors if we're getting poems
    // EFFECTS: instantiates a list of authors or poems in the Unity scene
    // MODIFIES: the _CHOOSEPOEM_ scene
    // REQUIRES: nothing
    private void DisplayItems()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        ArrayList abcList = new ArrayList();

        float yPos = -offset;
        int i = 0;

        if (isAuthorScreen)
        {
            foreach (int id in authIDs.Keys)
            {
                dict.Add(authIDs[id], id);
            }
        }
        else
        {
            foreach (int id in poemIDs.Keys)
            {
                dict.Add(poemIDs[id], id);
            }
        }

        foreach (string key in dict.Keys)
        {
            abcList.Add(key);
        }
        abcList.Sort();
        SetContentHeight(dict.Count);

        foreach (string name in abcList)
        {
            Quaternion rotation = listItemPrefab.transform.rotation;
            Vector3 position = new Vector3(0, yPos, 0);

            GameObject instListItem = Instantiate(listItemPrefab, position, rotation, this.transform);
            instListItem.name = "Item " + i;
            instListItem.transform.position = new Vector3(0, yPos, -1);
            instListItem.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasWidth, 40);
            instListItem.GetComponent<BoxCollider2D>().size = new Vector2(canvasWidth, 40);

            Transform caption = instListItem.transform.Find("Caption");
            caption.GetComponent<TextMesh>().text = name;
            caption.localPosition = new Vector3(-canvasWidth/2 + 10, caption.localPosition.y, caption.localPosition.z);

            Transform background = instListItem.transform.Find("Background");
            background.localScale = new Vector3(instListItem.GetComponent<RectTransform>().sizeDelta.x/10, 1, 4);

            Transform delineator = instListItem.transform.Find("Delineator");
            delineator.localScale = new Vector3(background.localScale.x, delineator.localScale.y, delineator.localScale.z);

            Transform checkmark = instListItem.transform.Find("Checkmark");
            checkmark.gameObject.SetActive(HasCompletedPoem(name));

            ChooseElement item = instListItem.GetComponent<ChooseElement>();
            item.id = dict[name];

            i++;
            yPos = yPos - (40/3);
        }
        abcList.Clear();
    }


    // TODO: Handle the case where two authors wrote a poem by the same name
    // EFFECTS: returns true if the user has completed the poem of this name
    // MODIFIES: nothing
    // REQUIRES: the name of the poem, the stats variable to be  initialized
    private bool HasCompletedPoem(string name)
    {
        if (isAuthorScreen)
        {
            return false;
        }
        else
        {
            if (stats == null)
            {
                return false;
            }
            for (int i = 0; i < stats.GetLength(0); i++)
            {
                if (stats[i, 1] == null) continue;
                if (stats[i, 1].Trim() == name)
                {
                    return true;
                }
            }
        }
        return false;
    }


    // EFFECTS: Sets the height of the Viewport to enable the whole list to be scrolled
    // MODIFIES: this
    // REQUIRES: nothing
    void SetContentHeight(int numItems)
    {
        RectTransform content = transform.GetComponent<RectTransform>();
        RectTransform scrollView = GameObject.Find("Canvas/Scroll View").GetComponent<RectTransform>();
        content.offsetMin = new Vector2(content.offsetMin.x, -40*numItems + scrollView.sizeDelta.y  + offset);
        content.offsetMax = new Vector2(content.offsetMax.x, 0);

        if (content.offsetMin.y > 0)
        {
            HideArrows();
        }
        else
        {
            ShowArrows();
        }

    }

    // EFFECTS: send the user to a new page displaying the poems under the selected author
    // MODIFIES: this, the "List Item" objects in the scene
    // REQUIRES: an author to have some poems in their text files?
    public void SelectAuthor(int authID, string authName)
    {
        currentAuth = authID;
        Utilities.authName = authName;

        // so poems fill up per author
        poemIDs.Clear();

        // swap headers
        GetComponentInParent<Canvas>().transform.Find("Headers").Find("Authors Title").gameObject.SetActive(false);
        GetComponentInParent<Canvas>().transform.Find("Headers").Find("Poems Title").gameObject.SetActive(true);
        isAuthorScreen = false;

        DestroyItems();
        GetPoemData(currentAuth);
        DisplayItems();
    }


    // EFFECTS: destroy all of the game objects that are children of the "List" object
    // MODIFIES: the "List" object's children
    // REQUIRES: nothing
    private void DestroyItems()
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
        poemIDs.Clear();
        int currentID = 0;

#if UNITY_EDITOR || PLATFORM_IOS
        string[] poemData;
        string dir = Application.streamingAssetsPath;
        string path = dir + "\\Authors\\" + authID + ".txt";

        //JUST FOR TESTING
        if (Utilities.GetOSVersion() == Utilities.OSVersion.MacOSX)
        {
            path = dir + "//Authors//" + authID + ".txt";
        }

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
                    string currentTitle = line.Substring(8).Trim();
                    //Debug.Log(currentTitle);
                    poemIDs.Add(currentID, currentTitle);

                    //foreach (int key in poemIDs.Keys)
                    //{
                    //    Debug.Log(poemIDs[key]);
                    //}
                }
            }
        }
#else
        Debug.Log("ENTERED NON-EDITOR CASE");
        var _path = "";

        //JUST FOR TESTING
        if (Utilities.GetOSVersion() == Utilities.OSVersion.Windows)
        {
            _path = Application.streamingAssetsPath + "\\Authors\\" + authID + ".txt";
        }
        else
        {
            _path = Application.streamingAssetsPath + "/Authors/" + authID + ".txt";
        }

        UnityWebRequest www = UnityWebRequest.Get(_path);
        www.Send();
        while (!www.isDone)
        {
        }
        string[] content = www.downloadHandler.text.Split('\n');
        for (int i = 0; i < content.Length; i++)
        {
            if (content[i].Contains("id ="))
            {
                currentID = int.Parse(content[i].Substring(5).Trim());
            }
            else if (content[i].Contains("title ="))
            {
                string currentTitle = content[i].Substring(8).Trim();
                if (!poemIDs.ContainsKey(currentID))
                {
                    poemIDs.Add(currentID, currentTitle);
                }
            }
        }
#endif
    }


    // EFFECTS: End this scene, and call the new _SCENE_ with the selected poem
    // MODIFIES: this, the _SCENE_ scene, the _CHOOSEPOEM_ scene
    // REQUIRES: currentAuth to have been set
    public void SelectPoem(int poemID, string poemTitle)
    {
        Utilities.poemTitle = poemTitle;
        Utilities.SetIDs(currentAuth, poemID);

        DestroyItems();
        SceneManager.LoadScene(1);
    }


    // EFFECTS: hide scroll arrows
    // MODIFIES: scroll arrow game objects
    // REQUIRES: nothing
    private void HideArrows()
    {
        GameObject.Find("Canvas/ScrollUp").SetActive(false);
        GameObject.Find("Canvas/ScrollDown").SetActive(false);
    }


    // EFFECTS: show scroll arrows
    // MODIFIES: scroll arrow game objects
    // REQUIRES: nothing
    private void ShowArrows()
    {
        GameObject.Find("Canvas/ScrollUp").SetActive(true);
        GameObject.Find("Canvas/ScrollDown").SetActive(true);
    }
}
