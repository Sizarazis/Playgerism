using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

	// Use this for initialization
	void Start () {
        halfHeightOfLine = (this.transform.localScale.y / 2) * 10;

        topMatch = false;
        bottomMatch = false;
    }


    // -- VARIABLES -- //
    public Slot inSlot;
    public Link inLink;

    private float halfHeightOfLine;
    private Vector3 screenPoint;
    private Vector3 offset;

    public int lineID;
    public bool bottomMatch;
    public bool topMatch;
    public bool isLast;


    // Update is called once per frame
    void Update () {

        //TODO: Remove this after link moving works
        UpdateMatches();
    }


    // EFFFECTS: updates the bottomMatch and topMatch variables according to the correct order of the lines
    // MODIFIES: this
    // REQUIRES: nothing
    public void UpdateMatches()
    {
        // NOTE: ONCE THE LINK MOVING WORKS, THINGS WILL NEVER GET OUT ORDER ONCE THEY'RE IN ORDER, SO I WON'T NEED TO KEEP UPDATING THIS
        // AT THIS POINT I'LL BE ABLE TO OPTIMIZE
        // I DON'T WANT TO DO IT YET THOUGH BECAUSE IT WOULD BE BROKEN UNTIL I GET THE LINKS WORK
        topMatch = false;
        bottomMatch = false;
        
        // Update topMatch
        if (inSlot.prevSlot != null)
        {
            if (lineID == inSlot.prevSlot.currentLine.lineID + 1)
            {
                topMatch = true;
            }
            else topMatch = false;
        }
        else
        {
            if (lineID == 0)
            {
                topMatch = false;
            }
            else topMatch = false;
        }

        // Update bottomMatch
        if (inSlot.nextSlot != null && lineID == inSlot.nextSlot.currentLine.lineID - 1)
        {
            bottomMatch = true;
        }
        else
        {
            if (isLast && inSlot.nextSlot == null)
            {
                bottomMatch = false;
            }
            else bottomMatch = false;
        }
    }


    // TODO: THIS DOESN'T WORK BECAUSE LINKS ARE BLOCKS, NOT JUST LINES. SO I NEED TO FIND THE NEXT LINK
    // EFFECTS: finds the closest slot to the current position of a line
    // MODIFIES: nothing
    // REQUIRES: a check to see if its looking up or down
    public Slot FindSlot(bool inMoveUp)
    {
        Slot currentSlot = inSlot;

        bool cont = true;
        while (cont)
        {
            if (currentSlot == null) Debug.Log("currentSlot not filled");

            // Move the line to the top spot if the current slot is the top slot, and its above the top slot
            if (inMoveUp == true && currentSlot.prevSlot == null  && this.transform.position.y >= currentSlot.transform.position.y && !inLink.lines.Contains(currentSlot.currentLine))
            {
                return currentSlot;
            }
            // Move the line to the bottom spot if the current slot is the bottom slot, and its below the bottom slot

            if (inMoveUp == false && currentSlot.nextSlot == null && this.transform.position.y <= currentSlot.transform.position.y && !inLink.lines.Contains(currentSlot.currentLine))
            {
                return currentSlot;
            }

            if (Mathf.Abs(currentSlot.transform.position.y - this.transform.position.y) <= halfHeightOfLine)
            {
                Link tempLink = currentSlot.currentLine.inLink;
                Line line;
                if (inMoveUp)
                {
                    line = (Line)tempLink.lines[0];
                }
                else
                {
                    line = (Line)tempLink.lines[tempLink.lines.Count - 1];
                }
                return line.inSlot;
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


    // EFFECTS: move the line to a slot above it, and update the other slots
    // MODIFIES: all lines and slots
    // REQUIRES: nothing
    //public void MoveUp()
    //{
    //    // Find the closest slot
    //    Slot newSlot = FindSlot(true);

    //    // Update all slots and lines
    //    ShiftLines(true, inSlot, newSlot);
    //}


    // EFFECTS: move the line to a slot below it, and update the other slots
    // MODIFIES: all lines and slots
    // REQUIRES: nothing
    //public void MoveDown()
    //{
    //    // Find the closest slot
    //    Slot newSlot = FindSlot(false);

    //    // Update all slots and lines
    //    ShiftLines(false, inSlot, newSlot);
    //}


    // TODO: ANIMATE!!
    // EFFECTS: set all the lines and slots to where they should be
    // MODIFIES: all lines and slots in the scene
    // REQUIRES: a bool to determine if a line is moving up or down, a new slot for this to go in, and the old slot, the number of items in the link we're moving
    //private void ShiftLines(bool inMoveUp, Slot oldSlot, Slot newSlot)
    //{
    //    Slot currentSlot = newSlot;
    //    Line currentLine = newSlot.currentLine;

    //    // Move the initial line
    //    newSlot.currentLine = oldSlot.currentLine;
    //    newSlot.currentLine.inSlot = newSlot;

    //    newSlot.currentLine.transform.SetPositionAndRotation(currentLine.transform.position, currentLine.transform.rotation);

    //    while (currentSlot != oldSlot)
    //    {
    //        if (inMoveUp == true)
    //        {
    //            // Save the nextSlot's previous line
    //            Line tempLine = currentSlot.nextSlot.currentLine;

    //            // Set the nextSlot's line to the currentSlot's line
    //            currentSlot.nextSlot.currentLine = currentLine;
    //            currentLine.inSlot = currentSlot.nextSlot;

    //            // Move the current line object to the correct position
    //            currentLine.transform.SetPositionAndRotation(new Vector3(currentLine.inSlot.transform.position.x,
    //                currentLine.inSlot.transform.position.y,
    //                currentLine.transform.position.z),
    //                currentLine.transform.rotation);

    //            // Set the nextSlot's previous line to the current line
    //            currentLine = tempLine;

    //            // Set the current slot to the next slot
    //            currentSlot = currentSlot.nextSlot;
    //        }
    //        else
    //        {
    //            // Save the nextSlot's previous line
    //            Line tempLine = currentSlot.prevSlot.currentLine;

    //            // Set the nextSlot's line to the currentSlot's line
    //            currentSlot.prevSlot.currentLine = currentLine;
    //            currentLine.inSlot = currentSlot.prevSlot;

    //            // Move the current line object to the correct position
    //            currentLine.transform.SetPositionAndRotation(new Vector3(currentLine.inSlot.transform.position.x,
    //                currentLine.inSlot.transform.position.y,
    //                currentLine.transform.position.z),
    //                currentLine.transform.rotation);

    //            // Set the nextSlot's previous line to the current line
    //            currentLine = tempLine;

    //            // Set the current slot to the next slot
    //            currentSlot = currentSlot.prevSlot;
    //        }
    //    }
    //}
}
