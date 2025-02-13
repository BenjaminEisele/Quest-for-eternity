using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChooseNewCardScript : MonoBehaviour
{
    [SerializeField]
    private GameObject displayCardReferenceGameobject;
    [SerializeField]
    private Transform displayCardLocator;
    [SerializeField]
    private DatabasePlayer databasePlayerAccess;
    public PlayerScript playerScriptAccess;
    public List<GameObject> displayCardList;
    


    private int displayCardCount = 0;


    private void Start()
    {
        Invoke("PreNewWaveEventSubscription", 1f);
    }

    private void PreNewWaveEventSubscription()
    {
        if (playerScriptAccess.isHost)
        {
            RefereeScript.instance.preNewWaveEvent += DisplayCards;
        }
    }

    public void ChooseOneCard(GameObject selfObject, int inputId)
    {
        displayCardCount--;
        Destroy(selfObject);
        databasePlayerAccess.gameObject.GetComponent<DeckManager>().discardedCardList.Add(inputId);
        if(RefereeScript.instance.singlePlayerMode)
        {
            if (displayCardCount <= 0)
            {
                DisplayCardsHidden();                
            }
        }
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
                    playerScriptAccess.isThisPlayersTurnToChoose = true;
                    if (displayCardCount <= 0)
                    {
                        if(!playerScriptAccess.isHost)
                        {
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
        displayCardList.Clear();
        if (playerScriptAccess.isHost)
        {
            RefereeScript.instance.CallEndTurnForBothPlayers();
        }
        else
        {
            RefereeScript.instance.CmdCallEndTurnForBothPlayers();
        }

        RefereeScript.instance.StartNextWaveInitalize();
    }

    public void DisplayCards()
    {
       /* if(RefereeScript.instance.myCoroutine != null)
        {
            StopCoroutine(RefereeScript.instance.myCoroutine);
            //RefereeScript.instance.myCoroutine = null;
        } */
        
        Vector3 newDisplayCardLocation = displayCardLocator.position;
        for (int i = 0; i < RefereeScript.instance.lootCardCount; i++)
        {
            displayCardCount++;
            int inputId = RefereeScript.instance.GetRandomNumber(i);
            GameObject displayCard = Instantiate(displayCardReferenceGameobject, newDisplayCardLocation, Quaternion.identity, transform);
            displayCard.GetComponent<DisplayCardScript>().playerScriptAccess = this.playerScriptAccess;
            displayCard.GetComponent<DisplayCardScript>().DisplayCardSetup(inputId);
            newDisplayCardLocation += new Vector3(1.5f, 0, 0);
            displayCardList.Add(displayCard);
        }
        RefereeScript.instance.lootCardCount = 0;
    }
}
