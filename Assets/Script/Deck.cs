using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour, DropZoneInterface {
    
    //return bool is not usefull in this case
    //used for set the cards' right visibility
    public bool check(Card c)
    {
        int children = transform.childCount;
        for (int i = 0; i < children; i++)
            transform.GetChild(i).GetComponent<Card>().setVisibility(false);
        return true;
    }

    public void clean()
    {
        int children = transform.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(transform.GetChild(0).gameObject);        
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }
}
