using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandScript : MonoBehaviour
{
    [SerializeField]
    GameObject baseCard;

    [SerializeField]
    Transform cardSpawnLocator;

    [SerializeField]
    int cardLimit;

    [SerializeField]
    FieldScript fieldScriptAccess;

    [SerializeField]
    TurnScript turnScriptAccess;

    [SerializeField]
    PlayerScript playerScriptAccess;

    [SerializeField]
    DeckManager deckManagerAccess;

    [SerializeField] // kodel sitas veikia tik su serializefield arba padarant list'a public?
    private List<CardScript> cardList;

    [SerializeField]
    int cardCount = 0;

    Vector3 cardPlacementVector;
    Coroutine handScriptDelayCoroutine;

    public bool canInteract;

    public bool isInQuickAttackMode;

    int cardDebt;
    public List<CardQueueUnit> cardQueDataList;
    int cardQueIndex;
    bool isHitrateAffected;
    float savedHitrateDelta;

    bool isFullRefill;
    private void Start()
    {
        RefereeScript.turnStartEvent += ActivateAllCardsEvent;
        RefereeScript.newWaveEvent += HandReset;
        RefereeScript.preNewWaveEvent += DisableAllCardsEvent;
        TurnScript.endTurnEvent += AddCardsEvent;
        TurnScript.restartGameEvent += HandReset;
        isInQuickAttackMode = false;
        cardCount = 0;
        cardDebt = 0;
        cardQueIndex = 0;
        canInteract = true;
        CardInstantiation();
        ActivateAllCardsEvent();
    }

    private void Update()
    {
        
        if(canInteract && playerScriptAccess.isThisPlayersTurn)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
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
                                   // Debug.Log("end turn by card");
                                    
                                    handScriptDelayCoroutine = StartCoroutine(EndTurnDelayCoroutine());
                                    //turnScriptAccess.EndPlayersTurn();   
                                }
                            }
                            //Debug.Log(hit.transform.parent.gameObject);
                            Destroy(hit.transform.parent.gameObject);
                            cardCount--;
                        }
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                AddCardsToHand(2);
            }
        }
    }
    public void DisableAllCardsEvent()
    {
        SetCardActivityStatus(false, 2);
    }
    public void ActivateAllCardsEvent()
    {
        
        Debug.Log("yo mama");
        if (playerScriptAccess.isThisPlayersTurn)
        {
            SetCardActivityStatus(true, 2);
        }
    }
    private void AddCardsEvent()
    {
        AddCardsToHand(0);
    }
    public void SetCardActivityStatus(bool desiredCardStatus, int inputCardType)
    {
        //Debug.Log("Activity status changed");
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
        fieldScriptAccess.FieldClearAndDealDamage(true);
        isInQuickAttackMode = false;
        SetCardActivityStatus(true, 0);
        RestoreAllOriginalHitrates();
        canInteract = true;
    }
    private IEnumerator EndTurnDelayCoroutine()
    {
        SetCardActivityStatus(false, 2);
        yield return new WaitForSeconds(0.75f);
        /*AddCardsToHand(0);
        turnScriptAccess.EndPlayersTurn();
        fieldScriptAccess.FieldClearAndDealDamage(true);*/
        //TurnScript.endTurnEvent();
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

    public void HitRateRestoriationMethod()
    {
        RestoreAllOriginalHitrates();
    }
    private void RestoreAllOriginalHitrates()
    {
        foreach (CardScript card in cardList)
        {
            card.RestroreOriginalHitrate();
        }
        ChangeAllVisualHitrates(true, 0);
    }
    public void ChangeAllVisualHitrates(bool shouldRestoreOriginal, float effectValue)
    {
        isHitrateAffected = !shouldRestoreOriginal;
        savedHitrateDelta = effectValue;
        foreach (CardScript card in cardList)
        {
           card.ChangeVisualCardHitrate(shouldRestoreOriginal, effectValue);
        }
    }

    public void HandReset()
    {
        foreach(CardScript card in cardList)
        {
            if(card != null)
            {
                Destroy(card.gameObject);
            }
            
        }
        if(handScriptDelayCoroutine != null)
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

    public void AddCardsToHand(int refillCount)
    {

        //Refill count should be 0 if we want to fill the hand until it has 5 cards 
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

                            //Debug.Log("Card debt added");
                            cardDebt++;
                           // cardCount++;
                        }
                    }         
                   
                }

                cardPlacementVector += new Vector3(2, 0, 0);
            }
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
                        Debug.Log("Card debt added");
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

                        Debug.Log("Card debt added");
                        cardDebt++;
                    }
                    cardPlacementVector += new Vector3(2, 0, 0);
                }
            }
        }
    }
    private void GenerateCard(Vector3 cardPlacementVectorReference, int cardIndex)
    {
        GameObject cardClone = Instantiate(baseCard, cardSpawnLocator.position + cardPlacementVectorReference, Quaternion.identity);
        cardClone.SetActive(true);

        cardClone.GetComponent<CardScript>().HandCardSetup(deckManagerAccess.deckCardList[deckManagerAccess.deckCardList.Count - 1]);
        deckManagerAccess.handCardList.Add(deckManagerAccess.deckCardList[deckManagerAccess.deckCardList.Count - 1]);
        deckManagerAccess.deckCardList.RemoveAt(deckManagerAccess.deckCardList.Count - 1);

        if(isHitrateAffected)
        {
            cardClone.GetComponent<CardScript>().ChangeVisualCardHitrate(false, savedHitrateDelta);
        }

        if (deckManagerAccess.deckCardList.Count <= 0)
        {
            canInteract = false;
            //SetCardActivityStatus(false, 2);

            deckManagerAccess.ResetDeckBegin();
            SetCardActivityStatus(false, 2);
        }
       

        cardCount++;
        if (cardIndex < 0)
        {
            cardList.Add(cardClone.GetComponent<CardScript>());
        }
        else
        {
            cardList[cardIndex] = cardClone.GetComponent<CardScript>();
        }
        cardClone.GetComponent<CardScript>().SetCardActiveStatus(turnScriptAccess.isPlayersTurn);       
    }

    public void DrawQueuedCards()
    {
        if (playerScriptAccess.isThisPlayersTurn) 
        {
            SetCardActivityStatus(true, 2);
        }

        if (cardDebt > 0)
        {
            //Debug.Log("reached this!");
            Debug.Log($"Card debt is {cardDebt}");
            foreach(CardQueueUnit queUnit in cardQueDataList)
            {
                if(!isFullRefill)
                {
                   // cardCount--;
                    GenerateCard(queUnit.QueuedVector, queUnit.QueuedIndex);
                }
                else
                {
                  //  cardCount--;
                    GenerateCard(queUnit.QueuedVector, queUnit.QueuedIndex);
                    if(cardCount >= cardLimit)
                    {
                        break;
                    }
                } 
                
            }
            cardQueDataList.Clear();
            cardDebt = 0;
            cardQueIndex = 0;
        }
    }
}
