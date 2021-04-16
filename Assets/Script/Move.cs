using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to store the moves
//not a Monobehavior
 
public class Move {

    public int points;
    public GameObject src;
    public GameObject dst;
    public List<Transform> cards;
    public bool newCard;

    
    public Move(List<Transform> cards, GameObject source, GameObject destination, int points, bool newCard)
    {
        this.cards = cards;
        this.src = source;
        this.dst = destination;
        this.points = points;
        this.newCard = newCard;
    }    
    
    
}
