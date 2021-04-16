using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//interface for all the drop zone on the table
// Hand, Deck, Columns, Stacks
public interface DropZoneInterface {

    //check the move condition 
    bool check(Card c);
    //used in the reset process
    void clean();
    //for the animation - not implemented
    Vector3 getPosition();
}
