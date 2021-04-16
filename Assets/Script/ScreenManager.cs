using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//class to fit the screen size - just Portrait supported
public class ScreenManager : MonoBehaviour {
    
    // Use this for initialization
    void Start () {

        StartCoroutine(setScreenSpacingLayout());        
    }    

    IEnumerator setScreenSpacingLayout()
    {
        //Card for counting the space needed, delete it at the end
        GameObject go;
        go = (GameObject)Instantiate(Resources.Load("Card"));             

        //Deck, Hand & Stacks have the same amount
        GameObject[] Panel = GameObject.FindGameObjectsWithTag("Deck");        
        go.transform.SetParent(Panel[0].transform);
        
        //to wait the aspect ratio fitter's job
        yield return new WaitForEndOfFrame();

        //find the hight of a card using <Image> component, the cards overlapped
        float space = go.GetComponent<RectTransform>().sizeDelta.y * -1;
        
        //Deck
        Panel[0].GetComponent<VerticalLayoutGroup>().spacing = space;
        //set into the PlayManager
        PlayManager.instance.deck = Panel[0];

        //Hand
        Panel = GameObject.FindGameObjectsWithTag("Hand");
        Panel[0].GetComponent<VerticalLayoutGroup>().spacing = space;
        //set into the PlayManager
        PlayManager.instance.hand = Panel[0];

        //Stacks
        Panel = GameObject.FindGameObjectsWithTag("Stack");
        foreach(GameObject p in Panel)
        {
            p.GetComponent<VerticalLayoutGroup>().spacing = space;
            //set into the PlayManager
            PlayManager.instance.stacks.Add(p);
        }
           

        //Columns
        Panel = GameObject.FindGameObjectsWithTag("Column");
        go.transform.SetParent(Panel[0].transform);

        //to wait the aspect ratio fitter's job
        yield return new WaitForEndOfFrame();

        //add a 0.07% to show the cards 
        space = go.GetComponent<RectTransform>().sizeDelta.y * -1.06f;        

        foreach (GameObject p in Panel)        
            p.GetComponent<VerticalLayoutGroup>().spacing = space;

        //copy the same setting to the empty object top layer
        GameObject tl = GameObject.FindGameObjectsWithTag("TopLayer")[0];
        tl.GetComponent<RectTransform>().sizeDelta = Panel[0].GetComponent<RectTransform>().sizeDelta;
        tl.GetComponent<VerticalLayoutGroup>().spacing = Panel[0].GetComponent<VerticalLayoutGroup>().spacing*1.1f;

        //a loop to set column in the right order
        for (int i = 0; i <7; i++)
        {
            foreach (GameObject p in Panel)
            {
                if(p.transform.GetSiblingIndex() == i)
                {
                    //set into the PlayManager
                    PlayManager.instance.columns.Add(p);
                    break;
                }
            }           
        }       

        Destroy(go);
    }
}
