using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollArrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

	// Use this for initialization
	void Start () {
        if (name == "ScrollUp")
        {
            isUp = true;
        }
        else
        {
            isUp = false;
        }

        isScroll = false;
	}


    // -- VARIABLES --
    public Transform content;

    private bool isUp;
    public bool isScroll;
    private float enterYPosition;
    private int moveSpeed = 12;


    // Update is called once per frame
    void Update()
    {
        if (!isActiveAndEnabled) isScroll = false;
        if (isScroll)
        {
            if (Input.touchCount > 0 || (Input.mousePresent && Input.anyKey))
            {
                Move();
            }
            else
            {
                isScroll = false;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isScroll = true;
        enterYPosition = eventData.position.y;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isUp && eventData.position.y <= enterYPosition)
        {
            isScroll = false;
        }
        else if (!isUp && eventData.position.y >= enterYPosition)
        {
            isScroll = false;
        }

        //looks like things aren't being reset on the pointer exit
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isScroll = false;
    }


    // EFFECTS: moves the content up or down
    // MODIFIES: scroll view/content
    // REQUIRES: this to be active and the input to be hovering over it
    private void Move()
    {
        RectTransform rt_content = content.GetComponent<RectTransform>();
        if (isUp)
        {
            // Move up
            if (!isAtTop())
            {
                rt_content.offsetMax = new Vector2(rt_content.offsetMax.x, rt_content.offsetMax.y - moveSpeed);
                rt_content.offsetMin = new Vector2(rt_content.offsetMin.x, rt_content.offsetMin.y - moveSpeed);
            }
        }
        else
        {
            // Move down
            if (!isAtBottom())
            {
                rt_content.offsetMax = new Vector2(rt_content.offsetMax.x, rt_content.offsetMax.y + moveSpeed);
                rt_content.offsetMin = new Vector2(rt_content.offsetMin.x, rt_content.offsetMin.y + moveSpeed);
            }
        }
    }


    // EFFECTS: if the content is as high as it can go, then return true, else true
    // MODIFIES: nothing
    // REQUIRES: nothing
    private bool isAtTop()
    {

        if (content.GetComponent<RectTransform>().offsetMax.y <= 0)
        {
            //Debug.Log("is at top.");
            return true;
        }

        //Debug.Log("is NOT at top.");
        return false;
    }


    // EFFECTS: if the content is as low as it can go, then return true, else true
    // MODIFIES: nothing
    // REQUIRES: nothing
    private bool isAtBottom()
    {
        if (content.GetComponent<RectTransform>().offsetMin.y >= 0)
        {
            //Debug.Log("is at bottom.");
            return true;
        }

        //Debug.Log("is NOT at bottom.");
        return false;
    }
}
