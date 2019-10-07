using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoosePoemArrows : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    // Use this for initialization
    void Start()
    {
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
    private bool isScroll;
    private int moveSpeed = 10;


    // Update is called once per frame
    void Update()
    {
        if (isScroll)
            Move();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isScroll = true;
    }

    public void OnPointerUp(PointerEventData eventData)
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
