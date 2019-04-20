﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour {

    // Use this for initialization
    void Start() {
        hasFirstLine = false;
        hasLastLine = false;
    }


    // -- VARIABLES -- //
    public ArrayList lines = new ArrayList();
    public bool hasFirstLine;
    public bool hasLastLine;

    private Vector3 mouseDown;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 relPos;


    // Update is called once per frame
    void Update() {
       
    }


    // EFFECTS: merges the two Link objects. The toMerge Link is destroyed, and it's LinkedList is added to the bottom of this one
    // MODIFIES: this, the toMerge Link
    // REQUIRES: another Link to merge
    public void MergeLinks(Link toMerge)
    {
        if (this == toMerge) return;

        Line[] temp = new Line[toMerge.lines.Count];
        int i = 0;
        foreach (Line t in toMerge.lines)
        {
            temp[i] = t;
            t.transform.SetParent(transform);
            i++;
        }

        foreach (Line line in temp)
        {
            lines.Add(line);
            line.inLink = this;
            toMerge.lines.Remove(line);
        }

        Destroy(toMerge.gameObject);
        SetColliderPos();
        SetColliderSize();
    }


    public void OnMouseDown()
    {
        relPos = transform.position;

        // Get the screen position of this object
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        // Mark the world's y-position before drag
        mouseDown = Camera.main.ScreenToWorldPoint(screenPoint);

        // Get the world position of the cursor relative to the world position of this object
        offset = transform.position -
            Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }


    public void OnMouseDrag()
    {
        // Get the position of the cursor according to the screen
        Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        // Get the new position of the object in the world according to the cursors offset to 
        // this object in the world
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;

        // Set the new position of this object
        transform.position = cursorPosition;
    }


    public void OnMouseUp()
    {
        //If this line moves out of its slot position, reset the lines and slots accordingly
        if (lines.Count > 0)
        {
            Line first = (Line)lines[0];
            Line last = (Line)lines[lines.Count - 1];

            if (Mathf.Abs(mouseDown.y - transform.position.y) > 5)
            {
                if (mouseDown.y >= transform.position.y)
                {

                    //account for moving the last 1 down
                    if (last.inSlot.nextSlot != null)
                    {
                        MoveLines(false);
                    }
                    else
                    {
                        transform.position = relPos; //HERE
                    }
                }
                else
                {
                    //account for moving the first 1 up
                    if (first.inSlot.relPosition != 0)
                    {
                        MoveLines(true);
                    }
                    else
                    {
                        transform.position = relPos; //AND HERE
                    }
                }
            }
            //Otherwise, move the line back to its starting position
            else
            {
                transform.position = relPos; //AND HERE
            }
        }
    }


    // EFFECTS: moves all the lines
    // MODIFIES: links and lines
    // REQUIRES: nothing
    public void MoveLines(bool moveUp)
    {
        Line firstLine  = (Line)lines[0];
        Line lastLine   = (Line)lines[lines.Count - 1];
        Slot newSlot;

        if (moveUp)
        {
            newSlot = firstLine.FindSlot(true);
            ShiftLines(true, newSlot, firstLine.inSlot, lastLine.inSlot);
        }
        else
        {
            newSlot = lastLine.FindSlot(false);
            ShiftLines(false, firstLine.inSlot, lastLine.inSlot, newSlot);
        }
        SetColliderPos();
        SetColliderSize();

    }


    public void SetColliderPos()
    {
        Vector2 outOffset = new Vector2();
        int newOffsetY = (transform.childCount - 1) * -5;
        outOffset.x = GetComponent<BoxCollider2D>().offset.x;
        outOffset.y = newOffsetY;
        GetComponent<BoxCollider2D>().offset = outOffset;

        Line first = (Line)lines[0];
        Vector3 position = new Vector3(0, first.inSlot.transform.position.y, -2);
        transform.position = position;

        SetLinePositions();
    }


    // EFFECTS: resize this' associated box collider to be size of all the lines in the link
    // MODIFIES: the size of this' box collider
    // REQUIRES: nothing
    public void SetColliderSize()
    {
        Vector2 outSize = new Vector2();
        int newSizeY = transform.childCount * 10;
        outSize.x = GetComponent<BoxCollider2D>().size.x;
        outSize.y = newSizeY;

        GetComponent<BoxCollider2D>().size = outSize;
    }

    public void SetLinePositions()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            Line line = (Line)lines[i];
            int yVal = -10 * i;
            Vector3 linePos = new Vector3(0, yVal, 2);

            line.transform.localPosition = linePos;
        }
    }


    // TODO: ANIMATE!!
    // EFFECTS: shifts all the lines into correct position after a move
    // MODIFIES: links and lines
    // REQUIRES: nothing
    public void ShiftLines(bool moveUp, Slot top, Slot mid, Slot bottom)
    {
        Slot ptr;
        Slot end;

        //MOVE UP
        if (moveUp)
        {
            //Shift everything between top and mid down by the size of the link
            ptr = top;
            end = mid;

            Line[] toMove;
            int moveBy = lines.Count;

            //Get number of lines to move
            int acc1 = 0;
            while (ptr != end)
            {
                acc1++;
                ptr = ptr.nextSlot;
            }
            toMove = new Line[acc1];

            //Get the lines to move
            ptr = top;
            int acc2 = 0;
            while (ptr != end)
            {
                toMove[acc2] = ptr.currentLine;
                acc2++;
                ptr = ptr.nextSlot;
            }

            //Move all lines
            Slot newSlot;
            for (int i = 0; i < toMove.Length; i++)
            {
                newSlot = top;

                //Find correct spot for this line
                for (int j = 0; j < moveBy; j++)
                {
                    newSlot = newSlot.nextSlot;
                }

                //Set this line to the correct slot
                toMove[i].inSlot = newSlot;
                newSlot.currentLine = toMove[i];

                toMove[i].inLink.SetColliderPos();
                moveBy++;
            }


            //Then, just place everything from mid to bottom at the top
            Slot toGo = top;
            for (int i = 0; i < lines.Count; i++)
            {
                Line line = (Line)lines[i];
                toGo.currentLine = line;
                line.inSlot = toGo;

                toGo = toGo.nextSlot;
            }
            SetColliderPos();
        }

        //MOVE DOWN
        else
        {
            //Shift everything between mid and bottom up by the size of the link
            ptr = bottom;
            end = mid;

            Line[] toMove;
            int moveBy = lines.Count;

            //Get number of things to move
            int acc1 = 0;
            while (ptr != end)
            {
                acc1++;
                ptr = ptr.prevSlot;
            }
            toMove = new Line[acc1];

            //Get the lines to move
            ptr = bottom;
            int acc2 = 0;
            while (ptr != end)
            {
                toMove[acc2] = ptr.currentLine;
                acc2++;
                ptr = ptr.prevSlot;
            }

            //Move all lines
            Slot newSlot;
            for (int i = 0; i < toMove.Length; i++)
            {
                newSlot = bottom;

                //Find correct spot for this line
                for (int j = 0; j < moveBy; j++)
                {
                    newSlot = newSlot.prevSlot;
                }

                //Set this line to the correct slot
                toMove[i].inSlot = newSlot;
                newSlot.currentLine = toMove[i];

                toMove[i].inLink.SetColliderPos();

                moveBy++;
            }

            //Then, just place everything from top to mid at the bottom
            Slot toGo = bottom;
            for (int i = 0; i < lines.Count; i++)
            {
                Line line = (Line)lines[(lines.Count - 1) - i];
                toGo.currentLine = line;
                line.inSlot = toGo;

                toGo = toGo.prevSlot;
            }
            SetColliderPos();
        }
    }
}
