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

    int displayCardCount = 0;

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
                    if (displayCardCount <= 0)
                    {
                        DisplayCardsHidden();
                    }
                    break;
                }
            }
        }
    }
    public void DisplayCardsHidden()
    {
        displayCardList.Clear();
        RefereeScript.instance.canTransferTurnToPlayer = true;
        //playerScriptAccess.EndTurnPlayerScript();

        RefereeScript.instance.CallEndTurnForBothPlayers();
        RefereeScript.instance.CallStartTurnEvent();
        RefereeScript.instance.StartNextWave(false);

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
