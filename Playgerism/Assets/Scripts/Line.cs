using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

	// Use this for initialization
	void Start () {
        _HEIGHT = 60;
        solved = false;

        upperArrow = GameObject.Find("Canvas/Scroll View/ScrollUp");
        lowerArrow = GameObject.Find("Canvas/Scroll View/ScrollDown");
        //scrollView = GameObject.Find("Canvas/Scroll View");
    }


    // -- VARIABLES -- //
    public int id;
    public string text;
    public Slot slot;
    public bool solved;

    private static float _HEIGHT;

    private Vector3 initPos;
    private Vector3 newPos;

    private Vector3 mouseDown;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 relPos;

    public GameObject upperArrow;
    public GameObject lowerArrow;
    //public GameObject scrollView;


    // Update is called once per frame
    void Update () {

    }


    public void OnMouseDown()
    {
        RevealScrollArrows();
        if (!solved)
        {
            relPos = transform.position;
            initPos = transform.localPosition;

            // Get the screen position of this object
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);

            // Mark the world's y-position before drag
            mouseDown = Camera.main.ScreenToWorldPoint(screenPoint);

            // Get the world position of the cursor relative to the world position of this object
            offset = transform.position -
                Camera.main.ScreenToWorldPoint(
                    new Vector3(200, Input.mousePosition.y, -8));
        }
        else
        {
            //scrollView.GetComponent<NoDragScrollRect>().vertical = true;
        }
    }


    public void OnMouseDrag()
    {
        if (!solved)
        {
            // Get the position of the cursor according to the screen
            Vector3 cursorScreenPoint = new Vector3(200, Input.mousePosition.y, 2);

            // Get the new position of the object in the world according to the cursors offset to 
            // this object in the world
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;

            // Set the new position of this object
            transform.position = new Vector3(cursorPosition.x, cursorPosition.y, -8);
        }
    }


    public void OnMouseUp()
    {
        HideScrollArrows();
        if (!solved)
        {
            newPos = transform.localPosition;

            //If this line moves out of its slot position, reset the lines and slots accordingly
            if (Mathf.Abs(newPos.y - initPos.y) > _HEIGHT/2)
            {
                if (newPos.y <= initPos.y)
                {
                    if (slot.next != null)
                    {
                        //Debug.Log("Moving down...");
                        MoveDown();
                    }
                    else
                    {
                        transform.localPosition = initPos;
                    }
                }
                else
                {
                    if (!slot.prev.line.solved)
                    {
                        //Debug.Log("Moving up...");
                        MoveUp();
                    }
                    else
                    {
                        transform.localPosition = initPos;
                    }
                }
            }
            else
            {
                transform.localPosition = initPos;
            }
        }
        else
        {
            //scrollView.GetComponent<NoDragScrollRect>().vertical = false;
        }
    }


    // EFFFECTS: updates the bottomMatch and topMatch variables according to the correct order of the lines
    // MODIFIES: this
    // REQUIRES: nothing
    public void UpdateMatches()
    {

        if (slot.position == 0)
        {
            solved = true;
        }
        else
        {
            if (slot.prev.line.solved && slot.position == id)
            {
                solved = true;
            }
        }
    }


    // EFFECTS: finds the closest slot to the current position of a line
    // MODIFIES: nothing
    // REQUIRES: a check to see if its looking up or down
    public Slot FindSlot(bool moveUp)
    {
        Slot[] slots = transform.parent.parent.Find("Slots").GetComponentsInChildren<Slot>();
        foreach (Slot item in slots)
        {
            if (Mathf.Abs(item.transform.localPosition.y - newPos.y) < _HEIGHT/2)
            {
                if (item.line.solved)
                {
                   // Debug.Log("find first non-solved");
                    // Find the first line that is not solved and say it's the closest
                    for (int i=0; i<slots.Length; i++)
                    {
                        if (!slots[i].line.solved)
                        {
                            return slots[i];
                        }
                    }
                }
                else
                {
                    return item;
                }
            }
            else if (!moveUp && item == slots[slots.Length-1])
            {
                return item;
            }
            else if (moveUp && newPos.y > (slots[0].transform.localPosition.y))
            {
                // Find the first line that is not solved and say it's the closest
                for (int j = 0; j < slots.Length; j++)
                {
                    if (!slots[j].line.solved)
                    {
                        return slots[j];
                    }
                }
            }
        }

        return null;
    }


    // EFFECTS: move the line to a slot above it, and update the other slots
    // MODIFIES: all lines and slots
    // REQUIRES: nothing
    public void MoveUp()
    {
        // Find the closest slot
        Slot newSlot = FindSlot(true);

        transform.localPosition = newSlot.transform.localPosition;

        // Update all slots and lines
        ShiftLines(true, slot, newSlot);
    }


    // EFFECTS: move the line to a slot below it, and update the other slots
    // MODIFIES: all lines and slots
    // REQUIRES: nothing
    public void MoveDown()
    {
        // Find the closest slot
        Slot newSlot = FindSlot(false);

        transform.localPosition = newSlot.transform.localPosition;

        // Update all slots and lines
        ShiftLines(false, slot, newSlot);
    }


    //TODO: ANIMATE!!
    // EFFECTS: set all the lines and slots to where they should be
    // MODIFIES: all lines and slots in the scene
    // REQUIRES: a bool to determine if a line is moving up or down, a new slot for this to go in, and the old slot, the number of items in the link we're moving
    private void ShiftLines(bool inMoveUp, Slot oldSlot, Slot newSlot)
    {
        Slot currentSlot = newSlot;
        Line currentLine = newSlot.line;

        // Move the initial line
        newSlot.line = oldSlot.line;
        newSlot.line.slot = newSlot;

        newSlot.line.transform.SetPositionAndRotation(currentLine.transform.position, currentLine.transform.rotation);

        while (currentSlot != oldSlot)
        {
            if (inMoveUp == true)
            {
                // Save the nextSlot's previous line
                Line tempLine = currentSlot.next.line;

                // Set the nextSlot's line to the currentSlot's line
                currentSlot.next.line = currentLine;
                currentLine.slot = currentSlot.next;

                // Move the current line object to the correct position
                currentLine.transform.SetPositionAndRotation(new Vector3(currentLine.slot.transform.position.x,
                    currentLine.slot.transform.position.y,
                    currentLine.transform.position.z),
                    currentLine.transform.rotation);

                // Set the nextSlot's previous line to the current line
                currentLine = tempLine;

                // Set the current slot to the next slot
                currentSlot = currentSlot.next;
            }
            else
            {
                // Save the nextSlot's previous line
                Line tempLine = currentSlot.prev.line;

                // Set the nextSlot's line to the currentSlot's line
                currentSlot.prev.line = currentLine;
                currentLine.slot = currentSlot.prev;

                // Move the current line object to the correct position
                currentLine.transform.SetPositionAndRotation(new Vector3(currentLine.slot.transform.position.x,
                    currentLine.slot.transform.position.y,
                    currentLine.transform.position.z),
                    currentLine.transform.rotation);

                // Set the nextSlot's previous line to the current line
                currentLine = tempLine;

                // Set the current slot to the next slot
                currentSlot = currentSlot.prev;
            }
        }
    }

    // TODO: ANIMATE!!! (FADE IN)
    // EFFECTS: reveal the scroll arrows
    // MODIFIES: the scroll arrow objects
    // REQUIRES: nothing
    private void RevealScrollArrows()
    {
        upperArrow.SetActive(true);
        upperArrow.GetComponent<ScrollArrow>().isScroll = false;
        lowerArrow.SetActive(true);
        lowerArrow.GetComponent<ScrollArrow>().isScroll = false;
    }


    // TODO: ANIMATE!!! (FADE OUT)
    // EFFECTS: hide the scroll arrows
    // MODIFIES: the scroll arrows
    // REQUIRES: nothing
    private void HideScrollArrows()
    {
        upperArrow.SetActive(false);
        lowerArrow.SetActive(false);
    }
}
