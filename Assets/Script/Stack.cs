using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Stack : MonoBehaviour, IDropHandler, DropZoneInterface
{
    //set it in the editor
    public Card.Seeds s;

    public void OnDrop(PointerEventData eventData)
    {
        Card c = eventData.pointerDrag.GetComponent<Card>();
        c.dstParent = this.gameObject;

    }

    public bool check(Card c)
    {
        //check the seed
        if(s == c.seed)
        {
            //the first card is the draw of the stack so must be skiped
            //the Ace case
            if (transform.childCount == 1)
            {
                
                if(c.value == 1)
                    return true;
            }
            //all the other cards
            else
            {
                Card lastCard = transform.GetChild(transform.childCount-1).GetComponent<Card>();

                if (lastCard.seed == c.seed)
                {
                    if (lastCard.value == c.value - 1)
                    {
                        return true;
                    }
                }
            }
        }       
               
        return false;
    }

    public void clean()
    {
        //skip the first child that is the background of the stack
        int children = transform.childCount;
        for (int i = 0; i < children-1; i++)
            DestroyImmediate(transform.GetChild(1).gameObject);
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public bool isComplete()
    {
        if(transform.childCount == 14)
        {
            return true;
        }
        return false;
    }
}
