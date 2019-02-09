using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

	// Use this for initialization
	void Start () {
        halfHeightOfLine = (this.transform.localScale.y / 2) * 10;
        score = 0;
    }

    public Slot inSlot;

    private float halfHeightOfLine;
    private Vector3 screenPoint;
    private Vector3 offset;

    //NOTE: this is set on instantiation
    public int lineID;
    public int[] truePlaces; //THIS NEEDS TO ALLOW FOR DUPLICATE LINES TO GO IN MORE THAN 1 SPOT

    public int inPlace;
    public int score;

    // Update is called once per frame
    void Update () {

    }

    // TODO: MOVE SOME OF THIS INTO STATEMANAGER
    // EFFECTS: Gets the score of the line according to how many correct neighbours it has and its slot
    // MODIFIES: this
    // REQUIRES: nothing
    // NOTE: Might want to include time taken as well
    //public void SetScore()
    //{
    //    score = 0;
    //    bool goodPlace = false;
    //    bool goodBottomNeighbour = false;
    //    bool goodTopNeighbour = false;

    //    //int[] byBottom = GetBottomLinePositions();
    //    //int[] byTop = GetTopLinePositions();

    //    for (int i = 0; i < truePlaces.Length; i++)
    //    {
    //        if (truePlaces[i] == inPlace)
    //        {
    //            goodPlace = true;
    //        }

    //        if (byBottom != null)
    //        {
    //            for (int j = 0; j < byBottom.Length; j++)
    //            {
    //                if (byBottom[j] == truePlaces[i] + 1)
    //                {
    //                    goodBottomNeighbour = true;
    //                }
    //            }
    //        }

    //        if (byTop != null)
    //        {
    //            for (int k = 0; k < byTop.Length; k++)
    //            {
    //                if (byTop[k] == truePlaces[i] - 1)
    //                {
    //                    goodTopNeighbour = true;
    //                }
    //            }
    //        }
    //    }

    //    if (goodPlace) score += 3;
    //    if (goodBottomNeighbour) score += 1;
    //    if (goodTopNeighbour) score += 1;

    //    Debug.Log("TruePlace: " + truePlaces[0]);
    //    Debug.Log("InPlace: " + inPlace);
    //    Debug.Log(score);
    //}

    ////Helper function for SetScore
    //private int[] GetBottomLinePositions()
    //{
    //    if (inSlot.nextSlot.currentLine == null)
    //    {
    //        return null;
    //    }
    //    else
    //    {
    //        return inSlot.nextSlot.currentLine.truePlaces;
    //    }
    //}

    ////Helper function for SetScore
    //private int[] GetTopLinePositions()
    //{
    //    if (inSlot.prevSlot == null)
    //    {
    //        return null;
    //    }
    //    else
    //    {
    //        return inSlot.prevSlot.currentLine.truePlaces;
    //    }
    //}

    private void OnMouseDown()
    {
        // Get the screen position of this object
        screenPoint = Camera.main.WorldToScreenPoint(this.transform.position);

        // Get the world position of the cursor relative to the world position of this object
        offset = this.transform.position - 
            Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    private void OnMouseDrag()
    {
        // Get the position of the cursor according to the screen
        Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        // Get the new position of the object in the world according to the cursors offset to 
        // this object in the world
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;

        // Set the new position of this object
        this.transform.position = cursorPosition;
    }

    private void OnMouseUp()
    {    
        // If this line moves out of its slot position, reset the lines and slots accordingly
        if (Mathf.Abs(inSlot.transform.position.y - this.transform.position.y) > halfHeightOfLine)
        {
            if (inSlot.transform.position.y >= this.transform.position.y)
            {
                // account for moving the last 1 down
                if (inSlot.nextSlot.currentLine != null)
                {
                    MoveDown();
                }
                else
                {
                    this.transform.position = new Vector3(inSlot.transform.position.x, inSlot.transform.position.y, this.transform.position.z);
                }
            }
            else
            {
                // account for moving the first 1 up
                if (inSlot.prevSlot != null)
                {
                    MoveUp();
                }
                else
                {
                    this.transform.position = new Vector3(inSlot.transform.position.x, inSlot.transform.position.y, this.transform.position.z);
                }
            }
        }

        // Otherwise, move the line back to its starting position
        else
        {
            this.transform.position = new Vector3(inSlot.transform.position.x, inSlot.transform.position.y, this.transform.position.z);
        }

        inPlace = inSlot.relPosition;
        SetScore();
    }

    // EFFECTS: Move the line to a slot above it, and update the other slots
    // MODIFIES: All lines and slots
    // REQUIRES: Nothing
    private void MoveUp()
    {
        // Find the closest slot
        Slot newSlot = FindSlot(true);

        // Update all slots and lines
        ShiftLines(true, inSlot, newSlot);
    }

    // EFFECTS: Move the line to a slot below it, and update the other slots
    // MODIFIES: All lines and slots
    // REQUIRES: Nothing
    void MoveDown()
    {
        // Find the closest slot
        Slot newSlot = FindSlot(false);

        // Update all slots and lines
        ShiftLines(false, inSlot, newSlot);
    }

    // EFFECTS: Finds the closest slot to the current position of a line
    // MODIFIES: Nothing
    // REQUIRES: A check to see if its looking up or down
    private Slot FindSlot(bool inMoveUp)
    {
        Slot currentSlot = inSlot;

        bool cont = true;
        while (cont)
        {
            // Move the line to the top spot if the current slot is the top slot, and its above the top slot
            if (inMoveUp == true && currentSlot.prevSlot == null  && this.transform.position.y >= currentSlot.transform.position.y)
            {
                return currentSlot;
            }

            // Move the line to the bottom spot if the current slot is the bottom slot, and its below the bottom slot
            if (inMoveUp == false && currentSlot.nextSlot.currentLine == null && this.transform.position.y <= currentSlot.transform.position.y)
            {
                return currentSlot;
            }

            if (Mathf.Abs(currentSlot.transform.position.y - this.transform.position.y) <= halfHeightOfLine)
            {
                return currentSlot;
            }
            else
            {
                if (inMoveUp == true)
                {
                    currentSlot = currentSlot.prevSlot;
                }
                else
                {
                    currentSlot = currentSlot.nextSlot;
                }
            }
        }
        return null;
    }

    // TODO: ANIMATE!!
    // EFFECTS: Set all the lines and slots to where they should be
    // MODIFIES: All lines and slots in the scene
    // REQUIRES: a bool to determine if a line is moving up or down, a new slot for this to go in, and the old slot
    private void ShiftLines(bool inMoveUp, Slot oldSlot, Slot newSlot)
    {
        Slot currentSlot = newSlot;
        Line currentLine = newSlot.currentLine;

        // Move the initial line
        newSlot.currentLine = oldSlot.currentLine;
        newSlot.currentLine.inSlot = newSlot;

        newSlot.currentLine.transform.SetPositionAndRotation(currentLine.transform.position, currentLine.transform.rotation);

        while (currentSlot != oldSlot)
        {
            if (inMoveUp == true)
            {
                // Save the nextSlot's previous line
                Line tempLine = currentSlot.nextSlot.currentLine;

                // Set the nextSlot's line to the currentSlot's line
                currentSlot.nextSlot.currentLine = currentLine;
                currentLine.inSlot = currentSlot.nextSlot;

                // Move the current line object to the correct position
                currentLine.transform.SetPositionAndRotation(new Vector3(currentLine.inSlot.transform.position.x, 
                    currentLine.inSlot.transform.position.y, 
                    currentLine.transform.position.z), 
                    currentLine.transform.rotation);

                // Set the nextSlot's previous line to the current line
                currentLine = tempLine;

                // Set the current slot to the next slot
                currentSlot = currentSlot.nextSlot;
            }
            else
            {
                // Save the nextSlot's previous line
                Line tempLine = currentSlot.prevSlot.currentLine;

                // Set the nextSlot's line to the currentSlot's line
                currentSlot.prevSlot.currentLine = currentLine;
                currentLine.inSlot = currentSlot.prevSlot;

                // Move the current line object to the correct position
                currentLine.transform.SetPositionAndRotation(new Vector3(currentLine.inSlot.transform.position.x,
                    currentLine.inSlot.transform.position.y,
                    currentLine.transform.position.z),
                    currentLine.transform.rotation);

                // Set the nextSlot's previous line to the current line
                currentLine = tempLine;

                // Set the current slot to the next slot
                currentSlot = currentSlot.prevSlot;
            }
        }
    }
}
