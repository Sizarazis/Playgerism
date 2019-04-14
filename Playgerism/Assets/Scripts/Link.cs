using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour {

    // Use this for initialization
    void Start() {
        hasFirstLine = false;
        hasLastLine = false;
        mouseDown = false;
    }


    // -- VARIABLES -- //
    public LinkedList<Line> lines = new LinkedList<Line>();
    public bool hasFirstLine;
    public bool hasLastLine;
    private bool mouseDown;
    

    // Update is called once per frame
    void Update() {

    }

    // TODO: Destroy empty Link objects (not necessary, but clutters)
    // EFFECTS: Merges the two Link objects. The toMerge Link is destroyed, and it's LinkedList is added to the bottom of this one
    // MODIFIES: this, the toMerge Link
    // REQUIRES: Another Link to merge
    public void MergeLinks(Link toMerge)
    {
        if (this == toMerge) return;

        Line[] temp = new Line[toMerge.lines.Count];
        int i = 0;
        foreach (Line t in toMerge.lines)
        {
            temp[i] = t;
            t.transform.SetParent(this.transform);
            i++;
        }

        foreach (Line line in temp)
        {
            lines.AddLast(line);
            line.inLink = this;
            toMerge.lines.Remove(line);
        }
    }

    // TODO
    // EFFECTS: If one of the line objects in link has OnMouseDown() this will loop through all of the line objects in this link
    // MODIFIES: The lines objects 
    // REQUIRES: Nothing
    public void HandleOnMouseDown()
    {
        mouseDown = true;
    }

    // TODO
    // EFFECTS: If one of the line objects moves then this will move all of them
    // MODIFIES: The lines objects
    // REQUIRES: Nothing
    public void HandleOnMouseDrag()
    {

    }

    // TODO
    // EFFECTS: If mouseDown is true and one of the line objects calls OnMouseUp(), then this will shift all the lines in the lines object
    // MODIFIES: the lines objects
    // REQUIRES: Nothing
    public void HandoleOnMouseUp()
    {
        mouseDown = false;
    }



    // TODO/NOTES:
    //  - Expand the rectange image according to how many lines are in the link (need to be able to delineate between different links)
    //  - Handle movement of the pack

    // this needs to move in chunks... maybe I move all movements from lines into this? A link of 1 would just need to display no graphic
    // but ughhhhhhhhhh that means I have to do the movement stuff again, and make it even more complicated

    // I should make a link for each line to start with
    // If they have neighbours I'll merge lists
}
