using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    void Update() {
    }


    // TODO: set parent's currentAuth or currentPoem when clicked
    //TODO: REPLACE WITH ONTOUCH WITH RAYCAST
    public void OnMouseDown()
    {
        // ANIMATE: Press down animation
    }

    //TODO: REPLACE WITH ONTOUCH WITH RAYCAST
    public void OnMouseUp()
    {
        Background header = GameObject.Find("Headers").GetComponent<Background>();
        Background footer = GameObject.Find("Footer").GetComponent<Background>();
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        if (EventSystem.current.currentSelectedGameObject != null)
            //&& EventSystem.current.currentSelectedGameObject.CompareTag("arrow"))
        {
            //Debug.Log("in case of arrow");
            return;
        }
        else if (header.inBackground == true || footer.inBackground == true)
        {
            return;
        }
        else
        {
            //Debug.Log("not in arrow case");
            if (isAuthor)
            {
                //Debug.Log("Select Author");
                list.SelectAuthor(id, text);
            }
            else
            {
                list.SelectPoem(id, text);
            }
        }
    }

}
