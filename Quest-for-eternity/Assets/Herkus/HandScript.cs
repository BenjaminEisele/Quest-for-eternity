using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandScript : MonoBehaviour
{
    [SerializeField]
    GameObject baseCard;

    [SerializeField]
    Transform cardSpawnLocator;

    int cardLimit;

    [SerializeField]
    FieldScript fieldScriptAccess;

    [SerializeField]
    TurnScript turnScriptAccess;

    [SerializeField]
    PlayerScript playerScriptAccess;

    [SerializeField]
    DeckManager deckManagerAccess;

    [SerializeField] 
    private List<CardScript> cardList;

    [SerializeField]
    int cardCount = 0;

    Vector3 cardPlacementVector;
    Coroutine handScriptDelayCoroutine;

    [HideInInspector]
    public bool canInteract;

    //[HideInInspector]
    public bool isInQuickAttackMode;

    [HideInInspector]
    public int utilityCount;
    int cardDebt;
    public List<CardQueueUnit> cardQueDataList;
    int cardQueIndex;
    bool isHitrateAffected;

    [HideInInspector]
    public bool canPlayUtility;
    float savedHitrateDelta;

    [HideInInspector]
    public int utlCardsPlayedForOtherPlayer;

    bool isFullRefill;
    private void Start()
    {
        cardLimit = 5;
        RefereeScript.turnStartEvent += ActivateAllCardsEvent;
        RefereeScript.preNewWaveEvent += DisableAllCardsEvent;
        TurnScript.endTurnEvent += AddCardsEvent;
        TurnScript.endTurnEvent += RebuildCardListLite;
        TurnScript.restartGameEvent += HandReset;
        TurnScript.restartGameEvent += RebuildCardListLite;
        isInQuickAttackMode = false;
        cardCount = 0;
        cardDebt = 0;
        cardQueIndex = 0;
        canInteract = true;
        deckManagerAccess.ShuffleCards(deckManagerAccess.deckCardList);
        CardInstantiation();
        ActivateAllCardsEvent();
        RebuildCardListLite();
        utlCardsPlayedForOtherPlayer = 0;
    }


    public void PlayCard()
    {
        if(canInteract && playerScriptAccess.isThisPlayersTurn)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.GetComponentInParent<CardScript>())
                {
                    if (hit.transform.GetComponentInParent<CardScript>().isClickable)
                    {
                        int clickedCardId = hit.transform.GetComponentInParent<CardScript>().myCardId;
                        deckManagerAccess.handCardList.Remove(clickedCardId);
                        deckManagerAccess.discardedCardList.Add(clickedCardId);

                        if (fieldScriptAccess.SpawnActiveCard(clickedCardId))
                        {
                            canInteract = false;
                            if (isInQuickAttackMode)
                            {
                                handScriptDelayCoroutine = StartCoroutine(QuickAttackModeCoroutine());
                            }
                            else
                            {
                                handScriptDelayCoroutine = StartCoroutine(EndTurnDelayCoroutine());
                            }
                        }
                        else
                        {
                            ShouldWeDisableCards();
                        }
                        RebuildCardList(hit.transform.root.gameObject);    
                    }
                }
            }
        }
    }

   
    public void RebuildCardListLite()
    {
        int interval = 90 / (cardCount + 1);
        for (int k = 0; k < cardList.Count; k++)
        {
            if (cardList[k] != null)
            {
                float myRotValue = 45 - interval * (k + 1);
                cardList[k].gameObject.transform.root.localEulerAngles = new Vector3(0, 0, myRotValue);
                cardList[k].gameObject.GetComponent<DragDrop>().cardPosition = cardList[k].gameObject.transform.localPosition;
            }
        }
    }
    private void RebuildCardList(GameObject inputGameobject)
    {
        for(int i = 0; i < cardList.Count; i++)
        {    
            if (GameObject.ReferenceEquals(inputGameobject.GetComponentInChildren<CardScript>().gameObject, cardList[i].gameObject))
            {
                Destroy(cardList[i].transform.root.gameObject);
                cardCount--;
                if(i < cardList.Count - 1)
                {
                    cardList[i] = cardList[i + 1];
                }
                else
                {
                    cardList[i] = null;
                }
               
                for(int j = i; j < cardList.Count;j++)
                {
                    if(j < cardList.Count - 1)
                    {
                        cardList[j] = cardList[j + 1];
                    }
                    else if(j >= cardList.Count - 1)
                    {
                        cardList[j] = null;
                    }
                }
                break;  
            }
        }
        int interval = 90 / (cardCount + 1);
        for (int k = 0; k < cardList.Count; k++)
        {
            if(cardList[k] != null)
            {
                float myRotValue = 45 - interval * (k + 1);
                cardList[k].gameObject.transform.root.localEulerAngles = new Vector3(0, 0, myRotValue);
                cardList[k].gameObject.GetComponent<DragDrop>().cardPosition = cardList[k].gameObject.transform.localPosition;
            }
        }
    }
    public void HitRateRestoriationMethod()
    {
        RestoreAllOriginalHitrates();
    }
    private void RestoreAllOriginalHitrates()
    {
        foreach (CardScript card in cardList)
        {
            if (card != null)
            {
                card.RestroreOriginalHitrate();
            }

        }
        ChangeAllVisualHitrates(true, 0);
    }
    public void ChangeAllVisualHitrates(bool shouldRestoreOriginal, float effectValue)
    {
        isHitrateAffected = !shouldRestoreOriginal;
        savedHitrateDelta = effectValue;
        foreach (CardScript card in cardList)
        {
            if (card != null)
            {
                card.ChangeVisualCardHitrate(shouldRestoreOriginal, effectValue);
            }
        }
    }
    public void ShouldWeDisableCards()
    {
        if (utilityCount > 2 && utlCardsPlayedForOtherPlayer > 2)
        {
            SetCardActivityStatus(false, 0);
            canPlayUtility = false;
        }
    }
    private void AddCardsEvent()
    {
        AddCardsToHand(0);
        utilityCount = 0;
    }
    public void DisableAllCardsEvent()
    {
        SetCardActivityStatus(false, 2);
    }
    public void ActivateAllCardsEvent()
    {
        if (playerScriptAccess.isThisPlayersTurn)
        {
            isInQuickAttackMode = false;
            canPlayUtility = true;
            SetCardActivityStatus(true, 2);
        }
        ShouldWeDisableCards();
    }   
    public void SetCardActivityStatus(bool desiredCardStatus, int inputCardType)
    {
        if(inputCardType == 0)
        {
            foreach (CardScript card in cardList)
            {
                if (card != null)
                {
                    if (!card.isActionCard)
                    {
                        card.SetCardActiveStatus(desiredCardStatus);
                    }
                }
            }
        }
        else if(inputCardType == 1)
        {
            foreach (CardScript card in cardList)
            {
                if (card != null)
                {
                    if (card.isActionCard)
                    {
                        card.SetCardActiveStatus(desiredCardStatus);
                    }
                }
            }
        }
        else if (inputCardType == 2)
        {
            foreach (CardScript card in cardList)
            {
                if (card != null)
                {
                    card.SetCardActiveStatus(desiredCardStatus);   
                }
            }
        }
    }

    private IEnumerator QuickAttackModeCoroutine()
    {
        yield return new WaitForSeconds(0.75f);
        isInQuickAttackMode = false;
        SetCardActivityStatus(true, 0);
        RestoreAllOriginalHitrates();
        canInteract = true;
    }
    private IEnumerator EndTurnDelayCoroutine()
    {
        SetCardActivityStatus(false, 2);
        yield return new WaitForSeconds(0.75f);
        turnScriptAccess.CallEndTurnEvent();
    }
    
    private void CardInstantiation()
    {
        Vector3 cardPlacementVector = new Vector3(1, 0, 0);
        for (int i = 0; i < cardLimit; i++)
        {
            GenerateCard(cardPlacementVector, -1);
            cardPlacementVector += new Vector3(2, 0, 0);
        }
    }

    public void AddCardsToHand(int refillCount)
    {
        int refillCycleCount;
        bool isFullRefill;
        if(refillCount <= 0)
        {
            canInteract = true;
            refillCycleCount = cardList.Count;
            isFullRefill = true;
        }
        else
        {
            refillCycleCount = cardList.Count + refillCount;
            isFullRefill = false;
        }
        this.isFullRefill = isFullRefill;
        cardDebt = 0;
        cardQueIndex = 0;
        cardPlacementVector = new Vector3(1, 0, 0);
        if (isFullRefill)
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                if (cardList[i] == null)
                {
                   
                    if (cardCount + cardDebt < cardLimit)
                    {
                        if (deckManagerAccess.deckCardList.Count > 0)
                        {
                            GenerateCard(cardPlacementVector, i);
                        }
                        else
                        {
                            cardQueDataList.Add(new CardQueueUnit());
                            cardQueDataList[cardQueIndex].QueuedVector = cardPlacementVector;
                            cardQueDataList[cardQueIndex].QueuedIndex = i;
                            cardQueIndex++;
                            cardDebt++;
                        }
                    }                   
                }
                cardPlacementVector += new Vector3(2, 0, 0);
            }
            DisableAllCardsEvent();
        }
        else
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                if (cardList[i] == null && refillCount > 0)
                {
                    if(deckManagerAccess.deckCardList.Count > 0)
                    {
                        GenerateCard(cardPlacementVector, i);
                        
                    }
                    else
                    {
                        cardQueDataList.Add(new CardQueueUnit());
                        cardQueDataList[cardQueIndex].QueuedVector = cardPlacementVector;
                        cardQueDataList[cardQueIndex].QueuedIndex = i;
                        cardQueIndex++;
                        cardCount++;
                        cardDebt++;
                    }
                    refillCount--;
                }
                cardPlacementVector += new Vector3(2, 0, 0);
            }
            if(refillCount > 0)
            {
                for (int i = 0; i < refillCount; i++)
                {
                    if (deckManagerAccess.deckCardList.Count > 0)
                    {
                        GenerateCard(cardPlacementVector, -1);
                    }
                    else
                    {
                        cardQueDataList.Add(new CardQueueUnit());
                        cardQueDataList[cardQueIndex].QueuedVector = cardPlacementVector;
                        cardQueDataList[cardQueIndex].QueuedIndex = i;
                        cardQueIndex++;
                        cardDebt++;
                    }
                    cardPlacementVector += new Vector3(2, 0, 0);
                }
            }
        }
    }
    private void GenerateCard(Vector3 cardPlacementVectorReference, int cardIndex)
    {
        GameObject cardClone = Instantiate(baseCard, cardSpawnLocator.position, Quaternion.identity);
        cardClone.SetActive(true);
        cardClone.GetComponentInChildren<CardScript>().HandCardSetup(deckManagerAccess.deckCardList[deckManagerAccess.deckCardList.Count - 1]);
        deckManagerAccess.handCardList.Add(deckManagerAccess.deckCardList[deckManagerAccess.deckCardList.Count - 1]);
        deckManagerAccess.deckCardList.RemoveAt(deckManagerAccess.deckCardList.Count - 1);
        if(isHitrateAffected)
        {
            cardClone.GetComponentInChildren<CardScript>().ChangeVisualCardHitrate(false, savedHitrateDelta);
        }
        if (deckManagerAccess.deckCardList.Count <= 0)
        {
            canInteract = false;
            deckManagerAccess.ResetDeckBegin();
            SetCardActivityStatus(false, 2);
        }
        cardCount++;
        if (cardIndex < 0)
        {
            cardList.Add(cardClone.GetComponentInChildren<CardScript>());
        }
        else
        {
            cardList[cardIndex] = cardClone.GetComponentInChildren<CardScript>();
        }
        cardClone.GetComponentInChildren<CardScript>().SetCardActiveStatus(turnScriptAccess.isPlayersTurn);      
    }

    public void DrawQueuedCards()
    {
        if (playerScriptAccess.isThisPlayersTurn)
        {
            SetCardActivityStatus(true, 2);
        }        
        if (cardDebt > 0)
        {
            foreach(CardQueueUnit queUnit in cardQueDataList)
            {
                if(!isFullRefill)
                {
                    GenerateCard(queUnit.QueuedVector, queUnit.QueuedIndex);
                }
                else
                {
                    GenerateCard(queUnit.QueuedVector, queUnit.QueuedIndex);
                    if(cardCount >= cardLimit)
                    {
                        break;
                    }
                } 
                
            }
            RebuildCardListLite();
            cardQueDataList.Clear();
            cardDebt = 0;
            cardQueIndex = 0;
        }
    }

    public void SendCardsOver()
    {
        if (canInteract && playerScriptAccess.isThisPlayersTurn)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {    
                if (hit.transform.GetComponentInParent<CardScript>().isClickable)
                {
                    utlCardsPlayedForOtherPlayer++;
                    int clickedCardId = hit.transform.GetComponentInParent<CardScript>().myCardId;
                    playerScriptAccess.PlayCardForOtherPlayer(clickedCardId);
                    deckManagerAccess.handCardList.Remove(clickedCardId);
                    deckManagerAccess.discardedCardList.Add(clickedCardId);
                    RebuildCardList(hit.transform.root.gameObject);
                }             
            }
        }
    }
    public void HandReset()
    {
        foreach (CardScript card in cardList)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        if (handScriptDelayCoroutine != null)
        {
            StopCoroutine(handScriptDelayCoroutine);
        }
        cardList.Clear();
        cardCount = 0;
        cardDebt = 0;
        cardQueIndex = 0;
        CardInstantiation();
        canInteract = true;
        isInQuickAttackMode = false;
    }

}
