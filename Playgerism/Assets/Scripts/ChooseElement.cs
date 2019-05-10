using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseElement : MonoBehaviour {

	// Use this for initialization
	void Start () {
        list = transform.GetComponentInParent<AuthorList>();
        isAuthor = list.isAuthorScreen;
        text = transform.Find("Caption").GetComponent<TextMesh>().text;
    }


    // -- VARIABLES --
    public bool isAuthor;
    public string text;
    public int id;

    private AuthorList list;


    // Update is called once per frame
    void Update () {
		
	}


    // TODO: set parent's currentAuth or currentPoem when clicked
    private void OnMouseDown()
    {
        // ANIMATE: Press down animation
    }


    private void OnMouseUp()
    {
        if (isAuthor)
        {
            list.SelectAuthor(id, text);
        }
        else
        {
            list.SelectPoem(id, text);
        }
    }

}
