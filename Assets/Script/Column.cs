using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Column : MonoBehaviour, IDropHandler, DropZoneInterface
{
    //set the destination in the card class
    public void OnDrop(PointerEventData eventData)
    {
        Card c = eventData.pointerDrag.GetComponent<Card>();
        c.dstParent = this.gameObject;
    }

    //called to flip the last card if is the case
    public bool UpdateCards()
    {
        if(transform.childCount > 0)
        {
            Card c = transform.GetChild(this.transform.childCount - 1).GetComponent<Card>();

            if (!c.visible)
            {
                c.Flip();
                return true;
            }
        }

        return false;
    }

    //called for Unflip the last card
    //the information is stored in the move's class
    public void UnFlip()
    {
        Card c = transform.GetChild(this.transform.childCount - 1).GetComponent<Card>();
        c.setVisibility(false);
    }

    public bool check(Card c)
    {
        //the column is empty and accept only K
        if (transform.childCount == 0)
        {
            if (c.value == 13)
                return true;
        }
        //check last card in the column
        else
        {
            Card lastCard = transform.GetChild(transform.childCount - 1).GetComponent<Card>();
            //xor operation to avoid the same color
            if (lastCard.isRed ^ c.isRed)
            {
                if (lastCard.value == (c.value + 1))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void clean()
    {
        int children = transform.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    //never used
    //for further usage like the animation
    public Vector3 getPosition()
    {
        if(transform.childCount != 0)
        {
            Card c = transform.GetChild(this.transform.childCount - 1).GetComponent<Card>();
            return c.transform.position;
        }

        return transform.position;

    }
}
