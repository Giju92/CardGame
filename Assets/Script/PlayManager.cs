using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour {

    //singleton
    public static PlayManager instance;

    //table reference
    public GameObject deck;
    public GameObject hand;
    public List<GameObject> stacks = new List<GameObject>();
    public List<GameObject> columns = new List<GameObject>();

    //object for the end game screen label
    public GameObject endScreen; 

    //Variable & delagate to trigger variables
    private int _points = 0;
    private int _moves = 0;
    public static event OnVariableChangeDelegate OnPointChange;
    public static event OnVariableChangeDelegate OnMovesChange;
    public delegate void OnVariableChangeDelegate(int newVal);
    public int Points
    {
        get
        {
            return _points;
        }
        set
        {
            if (_points == value) return;
            _points = value;
            if (_points < 0)
                _points = 0;
            if (OnPointChange != null)
                OnPointChange(_points);
        }
    }
    public int Moves
    {
        get
        {
            return _moves;
        }
        set
        {
            if (_moves == value) return;
            _moves = value;
            if (OnMovesChange != null)
                OnMovesChange(_moves);
        }
    }

    //stores all the moves done
    List<Move> listMoves = new List<Move>();

    //singleton initialization
    void Start()
    {
        instance = this;        
    }

    //new game routine 
    public void newGame()
    {
        //reset variable
        Moves = 0;
        Points = 0;
        listMoves = new List<Move>();

        //clean table
        deck.GetComponent<DropZoneInterface>().clean();
        hand.GetComponent<DropZoneInterface>().clean();

        foreach (GameObject go in stacks)
            go.GetComponent<DropZoneInterface>().clean();

        foreach (GameObject go in columns)
            go.GetComponent<DropZoneInterface>().clean();

        //new deck
        newDeck();

        //new table        
        newTable();
    }

    //create and shuffle the deck
    //set a random SibilingIndex to the cards
    void newDeck()
    {
        GameObject go;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= 13; j++)
            {
                go = (GameObject)Instantiate(Resources.Load("Card"));
                go.transform.SetParent(deck.transform);
                //reset the local scale to avoid the screen scale
                go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                go.GetComponent<Card>().setCard(i, j);
                //get a random number
                int cnt = Random.Range(0, deck.transform.childCount);
                go.transform.SetSiblingIndex(cnt);

            }

        }
    }

    //Gives the cards
    void newTable()
    {
        GameObject card;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < i; j++)
            {
                card = deck.transform.GetChild(deck.transform.childCount - 1).gameObject;
                card.transform.SetParent(columns[i].transform);
            }
            //last card must be visible
            card = deck.transform.GetChild(deck.transform.childCount - 1).gameObject;
            card.GetComponent<Card>().Flip();
            card.transform.SetParent(columns[i].transform);
        }
    }

    //set the cards in the TopLayer
    public void startMove(List<Transform> list)
    {
        foreach(Transform t in list)
        {
            t.SetParent(this.transform);
        }
    }

    //called by the dragged card
    public void dragMove(Vector2 v)
    {
        transform.position = v;
    }

    //Used to control the end move's result
    public void endMove(Card c)
    {
        //set out of the screen
        transform.position = new Vector2(Screen.width*2, Screen.height);        

        if (c.dstParent != null && c.dstParent != c.srcParent)
        {
            if (c.dstParent.GetComponent<DropZoneInterface>().check(c))
            {
                //move the cards
                foreach (Transform t in c.movingCards)
                    t.SetParent(c.dstParent.transform);

                //Update the table
                bool cardFlipped = false;
                if (c.srcParent.tag == "Column")
                    cardFlipped = c.srcParent.GetComponent<Column>().UpdateCards();

                //count the points
                int partialScore = getScore(c.srcParent, c.dstParent);
                if (cardFlipped)
                    partialScore += 5;

                Points += partialScore;

                //save the move
                listMoves.Add(new Move(c.movingCards, c.srcParent, c.dstParent, partialScore, cardFlipped));
                Moves++;

                //check the end
                if (c.dstParent.tag == "Stack")
                    checkEnd();

                return;
            }

        }

        //move the cards back
        foreach (Transform t in c.movingCards)
            t.SetParent(c.srcParent.transform);        
    }

    //called when a card is put in one of the stack
    public IEnumerator checkEnd()
    {
        //if ends the loop means that is complete
        foreach(GameObject go in stacks)
        {
            if (!go.GetComponent<Stack>().isComplete())
            {
                yield break;
            }               
        }

        //show the end screen and restart a new game
        endScreen.SetActive(true);
        yield return new WaitForSeconds(3);
        endScreen.SetActive(false);
        newGame();
    }

    //all the available combinations are mapped here
    public int getScore(GameObject src, GameObject dst)
    {
        int points = 0;
        
        if(src.tag == "Hand")
        {
            if(dst.tag == "Stack")
            {
                points = 10;
            }
            else if(dst.tag == "Column")
            {
                points = 5;
            }
        }
        else if (src.tag == "Column")
        {
            if (dst.tag == "Stack")
            {
                points = 10;
            }            
        }
        else if (src.tag == "Stack")
        {
            if (dst.tag == "Column")
            {
                points = -15;
            }            
        }

        return points;
    }

    //function to get the move before
    public void Undo()
    {
        if(listMoves.Count > 0)
        {
            Move lastMove = listMoves[listMoves.Count - 1];

            Points -= lastMove.points;
            Moves++;

            //to solve the situation Hand-Deck with empty deck
            if (lastMove.src.tag == "Hand" && lastMove.dst.tag == "Deck")
            {
                for (int i = lastMove.cards.Count - 1; 0 <= i; i--)
                {
                    //reordering starting from the last
                    lastMove.cards[i].transform.SetParent(lastMove.src.transform);                    
                }
            }
            else
            {
                if (lastMove.src.tag == "Column" && lastMove.newCard)
                    lastMove.src.GetComponent<Column>().UnFlip();

                //move back to the source
                foreach (Transform t in lastMove.cards)
                    t.SetParent(lastMove.src.transform);

            }

            //set the right visibility in the source and destination
            if (lastMove.src.tag == "Deck" || lastMove.src.tag == "Hand")
                lastMove.src.GetComponent<DropZoneInterface>().check(null);
            if (lastMove.dst.tag == "Deck" && lastMove.dst.tag == "Hand")
                lastMove.dst.GetComponent<DropZoneInterface>().check(null);       
            
            //remove the move
            listMoves.Remove(lastMove);
        }       
        
    }    

    //function for the deck behavior
    public void pickCard()
    {
        List<Transform> list = new List<Transform>();
        GameObject c;

        if (deck.transform.childCount > 0)
        {
            //take the last card
            c = deck.transform.GetChild(deck.transform.childCount - 1).gameObject;
            //lerp animation and insertion
            StartCoroutine(MoveCardandFlip(c,deck,hand,0.1f));                        
            
            //add move
            list.Add(c.transform);
            listMoves.Add(new Move(list, deck.gameObject, hand.gameObject, 0, false));
            Moves++;
        }
        else if (deck.transform.childCount == 0 && hand.transform.childCount > 0)
        {
            //move from hand to deck            
            for (int i = hand.transform.childCount -1; 0 <= i; i--)
            {
                //reordering starting from the last
                c = hand.transform.GetChild(i).gameObject;
                c.transform.SetParent(deck.transform);
                list.Add(c.transform);
                //flip
                c.GetComponent<Card>().setVisibility(false);
            }

            //add move
            listMoves.Add(new Move(list, hand.gameObject, deck.gameObject, -100, false));
            Moves++;
            Points -= 100;
        }

        //deck and hand empty

    }

    //general animation for move the acrd and flip
    //used for the deck
    public IEnumerator MoveCardandFlip(GameObject c, GameObject src, GameObject dst, float duration)
    {
        float time = duration;
        float elapsedTime = 0;
        Vector3 actualPosition = c.transform.position;

        Vector3 startPosition = src.GetComponent<DropZoneInterface>().getPosition();
        Vector3 endPosition = dst.GetComponent<DropZoneInterface>().getPosition();


        while (elapsedTime < time)
        {
            actualPosition.x = Mathf.Lerp(startPosition.x, endPosition.x, (elapsedTime / time));
            
            c.transform.position = actualPosition;

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        c.GetComponent<Card>().Flip();

        //put in the hand
        c.transform.SetParent(dst.transform);
    } 
}
