using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChooseNewCardScript : MonoBehaviour
{
    public GameObject displayCardReferenceGameobject;
    public Transform displayCardLocator;
    public DatabasePlayer databasePlayerAccess;
    public List<GameObject> displayCardList;
    public PlayerScript playerScriptAccess;


    public static ChooseNewCardScript instance;

    public int displayCardCount = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // DisplayCards();
        if(playerScriptAccess.isHost)
        {
            RefereeScript.preNewWaveEvent += DisplayCards;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            playerScriptAccess.EndTurnPlayerScript();
            //RefereeScript.instance.playerList[0].transform.root.GetComponentInChildren<TurnScript>().CallEndTurnEvent();
        }
    } 
    public void ChooseOneCard(GameObject selfObject, int inputId)
    {
        displayCardCount--;
        Destroy(selfObject);
        databasePlayerAccess.gameObject.GetComponent<DeckManager>().discardedCardList.Add(inputId);
        /*if(displayCardCount <= 0)
        {
            DisplayCardsHidden();
        } */
    }

    public void FindAndDestroyCard(int destroyableCardId)
    {
        foreach (GameObject displayCardObject in displayCardList)
        {
            if(displayCardObject != null)
            {
                if (displayCardObject.GetComponent<DisplayCardScript>().myCardId == destroyableCardId)
                {
                    displayCardCount--;
                    Destroy(displayCardObject);
                    //Debug.Log($"I destroyed card with the ID {displayCardObject.name}");
                    playerScriptAccess.isThisPlayersTurnToChoose = true;
                    Debug.Log("called outside");
                    if (displayCardCount <= 0)
                    {
                        Debug.Log("called inside");
                        if(!playerScriptAccess.isHost)
                        {
                            Debug.Log("called super inside");

                            //RefereeScript.instance.playerList[0].transform.root.GetComponentInChildren<ChooseNewCardScript>().DisplayCardsHidden();
                            RefereeScript.instance.playerList[0].DisplayCardsCallNest();
                        }
                        else
                        {
                            DisplayCardsHidden();
                        }
                        
                    }
                    break;
                }
            }
        }
    }

    public void DisplayCardsHidden()
    {
        Debug.Log("hello. my name is : " + transform.root.gameObject.name);

        displayCardList.Clear();
        //RefereeScript.instance.canTransferTurnToPlayer = true;
        //playerScriptAccess.EndTurnPlayerScript();

        //RefereeScript.instance.CallEndTurnForBothPlayers();
        //RefereeScript.instance.CallStartTurnEvent();
        if (playerScriptAccess.isHost)
        {
            RefereeScript.instance.CallEndTurnForBothPlayers();
        }
        else
        {
            RefereeScript.instance.CmdCallEndTurnForBothPlayers();
        }
        


        //transform.root.GetComponentInChildren<TurnScript>().CallEndTurnEvent();
        //playerScriptAccess.EndTurnPlayerScript();

        //RefereeScript.instance.playerList[0].transform.root.GetComponentInChildren<TurnScript>().CallEndTurnEvent();
        /*if (transform.root.GetComponentInChildren<PlayerScript>().isHost)
        {
            transform.root.GetComponentInChildren<TurnScript>().CallEndTurnEvent();
        }
        else
        {
            transform.root.GetComponentInChildren<TurnScript>().CallEndTurnEvent();
        }*/

    }


    public void DisplayCards()
    {
       
       // Debug.Log("display activated");
        Vector3 newDisplayCardLocation = displayCardLocator.position;
        for (int i = 0; i < 4; i++)
        {
            displayCardCount++;
            //int inputId = Random.Range(0, databasePlayerAccess.cardList.Count);
            int inputId = RefereeScript.instance.GetRandomNumber(i);
            Debug.Log($"my input id is {inputId}");
            GameObject displayCard = Instantiate(displayCardReferenceGameobject, newDisplayCardLocation, Quaternion.identity, transform);
            displayCard.GetComponent<DisplayCardScript>().playerScriptAccess = this.playerScriptAccess;
            displayCard.GetComponent<DisplayCardScript>().DisplayCardSetup(inputId);
            newDisplayCardLocation += new Vector3(2.5f, 0, 0);
            displayCardList.Add(displayCard);
        }
    }
}
