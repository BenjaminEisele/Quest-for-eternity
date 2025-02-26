using UnityEngine;
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

    [SerializeField] SoundFXManager soundFXManager;

    private int displayCardCount = 0;
    public bool isInLootingPhase = false;

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

    public void ChangeGlowEffectStatus(bool desiredActivation)
    {
        Debug.Log(transform.root.gameObject.name);
        Debug.Log(desiredActivation);
        foreach (GameObject displayCard in displayCardList)
        {
            Debug.Log("foreach");
            if(displayCard != null)
            {
                Debug.Log($"display card not null, setting outline to {desiredActivation}");
                displayCard.GetComponent<DisplayCardScript>().glowObject.SetActive(desiredActivation);
            }
            
        }
    }
    public void ChooseOneCard(GameObject selfObject, int inputId)
    {
        if (RefereeScript.instance.glowCoroutine != null)
        {
            Debug.Log("Stop Coroutine");
            StopCoroutine(RefereeScript.instance.glowCoroutine);
        }
        soundFXManager.DrawSound();
        displayCardCount--;
        Destroy(selfObject);
        databasePlayerAccess.gameObject.GetComponent<DeckManager>().discardedCardList.Add(inputId);
        RefereeScript.instance.databaseMultiplayerAccess.genericLootList.Remove(inputId);
        if (displayCardCount <= 0)
        {
            playerScriptAccess.StartTurnPlayerScript();
            isInLootingPhase = false;
        }
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
                    ChangeGlowEffectStatus(true);
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
                        isInLootingPhase = false;
                        playerScriptAccess.EndTurnPlayerScript();
                    }
                    break;
                }
            }
        }
    }
    public void DisplayCardsHidden()
    {
        isInLootingPhase = false;
        RefereeScript.instance.canTransferTurnToPlayer = true;
        transform.root.GetComponentInChildren<HandScript>().canInteract = true;

        displayCardList.Clear();
        /*if (playerScriptAccess.isHost)
        {
            RefereeScript.instance.CallEndTurnForBothPlayers();
        }
        else
        {
            RefereeScript.instance.CmdCallEndTurnForBothPlayers();
        }*/
        RefereeScript.instance.StartNextWaveInitalize();
    }

    public void DisplayCards()
    {
        isInLootingPhase = true;
        soundFXManager.LootAppearsSound();
        Vector3 newDisplayCardLocation = displayCardLocator.position;
        for (int i = 0; i < RefereeScript.instance.lootCardCount; i++)
        {
            displayCardCount++;
            int inputId = RefereeScript.instance.GetRandomNumber(i);
            GameObject displayCard = Instantiate(displayCardReferenceGameobject, newDisplayCardLocation, Quaternion.identity, transform);
            displayCard.GetComponent<DisplayCardScript>().playerScriptAccess = this.playerScriptAccess;
            displayCard.GetComponent<DisplayCardScript>().DisplayCardSetup(inputId);
            newDisplayCardLocation += new Vector3(1.25f, 0, 0);
            displayCardList.Add(displayCard);
        }
        RefereeScript.instance.lootCardCount = 0;
    }
}
