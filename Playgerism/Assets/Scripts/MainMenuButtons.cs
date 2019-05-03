using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetButtonType();
	}
	

    // -- VARIABLES --
    private enum ButtonType {
        NEW_GAME,
        RANDOM_START,
        STATS,
        HELP,
        LEGAL
    }
    private ButtonType type;


	// Update is called once per frame
	void Update () {
		
	}


    // EFFECTS: gets the type of button this is
    // MODIFIES: this
    // REQUIRES: nothing
    private void GetButtonType()
    {
        string text = transform.Find("Text").GetComponent<TextMesh>().text;

        switch(text)
        {
            case "New Game":
                type = ButtonType.NEW_GAME;
                break;
            case "Random Start":
                type = ButtonType.RANDOM_START;
                break;
            case "Stats":
                type = ButtonType.STATS;
                break;
            case "Help":
                type = ButtonType.HELP;
                break;
            case "Legal":
                type = ButtonType.LEGAL;
                break;
            default:
                type = ButtonType.NEW_GAME;
                break;
        }
    }


    // TODO: ANIMATE!!!
    private void OnMouseDown()
    {

    }


    // NOTE: if you move off of the button and mouseUp, it will still click that button (I don't want this)
    private void OnMouseUp()
    {
        switch(type)
        {
            case ButtonType.NEW_GAME:
                break;
            case ButtonType.RANDOM_START:
                SceneManager.LoadScene("_SCENE_", LoadSceneMode.Additive);
                break;
            case ButtonType.STATS:
                break;
            case ButtonType.HELP:
                break;
            case ButtonType.LEGAL:
                break;
            default:
                break;
        }
    }
}
