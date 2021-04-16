using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum Seeds { hearts, diamonds, clubs, spades };

    public Seeds seed;
    public int value;
    public bool visible = false;
    public bool isRed;

    public GameObject srcParent = null;
    public GameObject dstParent = null;
    public List<Transform> movingCards = null;
        
    //used to make a move
    public void setParent(GameObject go)
    {
        if(go != dstParent)
        {
            PlayManager.instance.getScore(dstParent, go);

            dstParent = go;
        }        
    }

    //create the card using seed and number, default visibility = false
    public void setCard(int s, int v)
    {
        //set the properties
        seed = (Seeds)s;
        value = v;

        Color c;
        string str;

        if ((Seeds)s == Seeds.hearts || (Seeds)s == Seeds.diamonds)
        {
            c = new Color(1, 0, 0);
            str = "Sprite/Figure/Red/";
            isRed = true;
        }
        else
        {
            c = new Color(0, 0, 0);
            str = "Sprite/Figure/Black/";
            isRed = false;
        }

        //initialize the images
        Sprite img = Resources.Load<Sprite>("Sprite/Seed/" + seed.ToString());
        
        //small & center seed
        transform.GetChild(0).GetComponent<Image>().sprite = img;

        if (v>=1 && v <= 10)
        {  
            if (v == 1)
            {
                transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Number/A");
            }
            else
            {
                transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Number/" + v);
            }       
            
        } 
        //Jack       
        else if (v == 11)
        {
            transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Number/J");
            img = Resources.Load<Sprite>(str + "J");
        }
        //Queen
        else if (v == 12)
        {
            transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Number/Q");
            img = Resources.Load<Sprite>(str + "Q");
        }
        //King
        else if (v == 13)
        {
            transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Number/K");
            img = Resources.Load<Sprite>(str + "K");
        }

        //set color
        transform.GetChild(1).GetComponent<Image>().color = c;
        //central image
        transform.GetChild(2).GetComponent<Image>().sprite = img;
               
        setVisibility(false);
    }

    public void setVisibility(bool b)
    {
        visible = b;
        
        if (b)
        {
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/front");
        }
        else
        {
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/retro");
        }

        setAlpha(transform.GetChild(0).GetComponent<Image>());
        setAlpha(transform.GetChild(1).GetComponent<Image>());
        setAlpha(transform.GetChild(2).GetComponent<Image>());

        GetComponent<CanvasGroup>().alpha = 1;
    }

    //support function for setVisibility
    //set alpha value and keep the color  
    private void setAlpha(Image i)
    {
        Color c = i.color;

        if (visible)
          c.a = 255;        
        else
          c.a = 0;
 
        i.color = c;
    }

    //search the cards to be moved
    public void OnBeginDrag(PointerEventData eventData)
    {
        //clean the variable       
        dstParent = null;

        if (visible)
        {
            //set the source
            srcParent = this.transform.parent.gameObject;

            //stores all the cards to be moved in the list
            movingCards = new List<Transform>();
            int index = transform.GetSiblingIndex();            
            while (index < transform.parent.childCount)
            {
                movingCards.Add(transform.parent.GetChild(index).transform);
                index++;
            }

            PlayManager.instance.startMove(movingCards);

            //to disable the raycast block in order to drop object
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

    }

    //send the position to the PlayManager during the drag
    public void OnDrag(PointerEventData eventData)
    {
        if (visible)
        {
            PlayManager.instance.dragMove(eventData.position);
        }       
    }
       
    //triggered when the drag ends, always after the onDrop
    public void OnEndDrag(PointerEventData eventData)
    {
        if (visible)
        {
            //send info to the manager
            PlayManager.instance.endMove(this);

            //enable the block raycast
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }       

    //starts the animation
    public void Flip()
    {
        GetComponent<Animator>().Play("Flip");
    }

    //used by the animation
    public void Turn()
    {
        setVisibility(!visible);
    }

}
