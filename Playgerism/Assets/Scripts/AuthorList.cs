using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class AuthorList : MonoBehaviour {

	// Use this for initialization
	void Start () {
        canvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x;
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

    private float canvasWidth;
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

        float yPos = 0;
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

            ChooseElement item = instListItem.GetComponent<ChooseElement>();
            item.id = dict[name];   

            i++;
            yPos = yPos - (40/3);
        }
    }


    // TODO: Test
    // EFFECTS: Sets the height of the Viewport to enable the whole list to be scrolled
    // MODIFIES: this
    // REQUIRES: nothing
    void SetContentHeight(int numItems)
    {
        RectTransform content = transform.GetComponent<RectTransform>();
        RectTransform scrollView = GameObject.Find("Canvas/Scroll View").GetComponent<RectTransform>();
        content.offsetMin = new Vector2(content.offsetMin.x, -40*numItems + scrollView.sizeDelta.y);
        content.offsetMax = new Vector2(content.offsetMax.x, 0);

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

        GetPoemData(authID);
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
        int currentID = 0;

#if UNITY_EDITOR
        string[] poemData;
        string dir = Application.streamingAssetsPath;
        string path = dir + "\\Authors\\" + authID + ".txt";

        ////JUST FOR TESTING
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
                    poemIDs.Add(currentID, currentTitle);
                }
            }
        }
#endif

        var _path = Application.streamingAssetsPath + "/Authors/" + Utilities.authID + ".txt";
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
                poemIDs.Add(currentID, currentTitle);
            }
        }
    }


    // EFFECTS: End this scene, and call the new _SCENE_ with the selected poem
    // MODIFIES: this, the _SCENE_ scene, the _CHOOSEPOEM_ scene
    // REQUIRES: currentAuth to have been set
    public void SelectPoem(int poemID, string poemTitle)
    {
        Utilities.poemTitle = poemTitle;
        Utilities.SetIDs(currentAuth, poemID);
        SceneManager.LoadScene(1);
    }
}
