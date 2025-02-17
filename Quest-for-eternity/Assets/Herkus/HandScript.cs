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
    DatabasePlayer databasePlayerAccess;

    [SerializeField]
    SceneObjectDatabase sceneObjectAccess;

    [SerializeField]
    int cardCount = 0;

    Vector3 cardPlacementVector;
    Coroutine handScriptDelayCoroutine;

    [HideInInspector]
    public bool canInteract;

    [HideInInspector]
    public bool isInQuickAttackMode;

    public bool isInLongShotMode;

    public bool isInMergeMode;
    //[HideInInspector]
    public int utilityCount;
    public int utilityLimit;
    int cardDebt;
    public List<CardQueueUnit> cardQueDataList;
    int cardQueIndex;
    bool isHitrateAffected;
    
    [HideInInspector]
    public bool canPlayUtility;
    float savedHitrateDelta;

    [HideInInspector]
    public int utlCardsPlayedForOtherPlayer;

    [SerializeField]
    GameObject damageSliderObject;

    bool isFullRefill;

    public bool isInDamageSliderMode;
    public int clickedCardId;
    private void Start()
    {
        utilityLimit = 3;
        cardLimit = 5;
        Invoke("SubscriptionInvokeHand", 1f);
        turnScriptAccess.endTurnEvent += AddCardsEvent;
        turnScriptAccess.endTurnEvent += RebuildCardListLite;
        turnScriptAccess.endTurnEvent += ResetQuickAttackMode;
        turnScriptAccess.restartGameEvent += HandReset;
        turnScriptAccess.restartGameEvent += RebuildCardListLite;
        damageSliderObject.SetActive(false);
        isInQuickAttackMode = false;
        isInDamageSliderMode = false;
        isInMergeMode = false;
        isInLongShotMode = false;
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

    private void SubscriptionInvokeHand()
    {
        RefereeScript.instance.turnStartEvent += NewTurnHandLogic;
        RefereeScript.instance.preNewWaveEvent += DisableAllCardsEvent;
    }

    public void PlayCard(Transform card)
    {
        if(canInteract && playerScriptAccess.isThisPlayersTurn)
        {           
            if (card.GetComponentInParent<CardScript>())
            {
                if (card.GetComponentInParent<CardScript>().isClickable)
                {
                    clickedCardId = card.GetComponentInParent<CardScript>().myCardId;
                    deckManagerAccess.handCardList.Remove(clickedCardId);
                    deckManagerAccess.discardedCardList.Add(clickedCardId);

                    if (isInMergeMode)
                    {
                        fieldScriptAccess.InputCardForMerging(clickedCardId);
                    }                   
                    else if (fieldScriptAccess.SpawnActiveCard(clickedCardId, false))
                    {
                        canInteract = false;
                        if (isInQuickAttackMode)
                        {
                            handScriptDelayCoroutine = StartCoroutine(QuickAttackModeCoroutine());
                        }
                        else
                        {
                            handScriptDelayCoroutine = StartCoroutine(EndTurnDelayCoroutine(clickedCardId));
                        }
                    }
                    else
                    {
                        ShouldWeDisableCards();
                    }
                    RebuildCardList(card.root.gameObject);    
                }
            }           
        }
    }

    public void MergedCardExecution(int firstInput, int secondInput)
    {
        isInMergeMode = false;
        handScriptDelayCoroutine = StartCoroutine(MergedCoroutine(firstInput, secondInput));
    }
    public void CustomAttackExecution()
    {
        SetCardActivityStatus(false, 2);
        damageSliderObject.SetActive(true);
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

    private void NewTurnHandLogic()
    {
        utilityLimit = 3;
        fieldScriptAccess.mergeIdList.Clear();
        isInMergeMode = false;
        isInDamageSliderMode = false;
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
        if (utilityCount > 5 && utlCardsPlayedForOtherPlayer > 2)
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
        playerScriptAccess.isPlayersTurnLocal = false;

    }
    public void ActivateAllCardsEvent()
    {
        if (playerScriptAccess.isThisPlayersTurn && playerScriptAccess.isPlayerAlive)
        {
            playerScriptAccess.isPlayersTurnLocal = true;
            if (isInQuickAttackMode && CountActionCards() > 0)
            {
                canPlayUtility = false;
                SetCardActivityStatus(true, 1);
            }
            else if(isInLongShotMode)
            {
                SetCardActivityStatus(true, 0);
                isInLongShotMode = false;
            }
            else if(isInMergeMode)
            {
                if (CountActionCards() > 1)
                {
                    canPlayUtility = false;
                    SetCardActivityStatus(true, 1);
                }
                else
                {
                    canPlayUtility = true;
                    SetCardActivityStatus(true, 2);
                    isInMergeMode = false;
                }
            }
            else
            {
                canPlayUtility = true;
                SetCardActivityStatus(true, 2);
                RestoreAllOriginalHitrates();
            }
        }
        ShouldWeDisableCards();
    }
    public void ResetQuickAttackMode()
    {
        isInQuickAttackMode = false;
    }
    private int CountActionCards()
    {
        int actionCardCount = 0;
        foreach (CardScript card in cardList)
        {
            if (card != null)
            {
                if (card.isActionCard)
                {
                    actionCardCount++;
                }
            }
        }
        return actionCardCount;
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
        playerScriptAccess.DealDamagePlayerScript(false, false, 0, false, true);
        isInQuickAttackMode = false;
        SetCardActivityStatus(true, 0);
        RestoreAllOriginalHitrates();
        canInteract = true;
    }
    private IEnumerator EndTurnDelayCoroutine(int inputCardId)
    {
        SetCardActivityStatus(false, 2);
        yield return new WaitForSeconds(0.75f);
        ActionCardEffectActivation(inputCardId);
        if(!isInDamageSliderMode)
        {
            turnScriptAccess.CallEndTurnEvent();
        }
        else
        {
            damageSliderObject.SetActive(true);
        }
        
    }

    private IEnumerator MergedCoroutine(int firstId, int secondId)
    {
        SetCardActivityStatus(false, 2);
        yield return new WaitForSeconds(0.75f);
        ActionCardEffectActivation(firstId);
        ActionCardEffectActivation(secondId);
        turnScriptAccess.CallEndTurnEvent();
    }
    private void ActionCardEffectActivation(int inputCardId)
    {
        Action actionCardAccess = databasePlayerAccess.cardList[inputCardId] as Action;
        foreach (EffectUnit myEffectUnit in actionCardAccess.actionEffectUnitList)
        {
            if (myEffectUnit.shouldActivateNow)
            {
                myEffectUnit.myEffect.UseEffect<GameObject>(RefereeScript.instance.chosenEnemyId, myEffectUnit.effectValue, sceneObjectAccess.gameObject);
            }
        }
    }
    public void DelayedActionCardEffectActivation()
    {
        if(clickedCardId != -1)
        {
            Action actionCardAccess = databasePlayerAccess.cardList[clickedCardId] as Action;
            if(actionCardAccess)
            {
                foreach (EffectUnit myEffectUnit in actionCardAccess.actionEffectUnitList)
                {
                    if (!myEffectUnit.shouldActivateNow)
                    {
                        myEffectUnit.myEffect.UseEffect<GameObject>(RefereeScript.instance.chosenEnemyId, myEffectUnit.effectValue, sceneObjectAccess.gameObject);
                    }
                }
                clickedCardId = -1;
            }
        } 
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
                        //cardCount++;
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

    private int CalculateCardIndex()
    {
        int returnIndex = 0;
        foreach(CardScript card in cardList)
        {
            if(card != null)
            {
                returnIndex++;
            }
            else
            {
                break;
            }
        }
        Debug.Log($"my index is {returnIndex}");
        return returnIndex;
    } 
    public void DrawQueuedCards()
    {
        Debug.Log($"card debt is : {cardDebt}");
        if (playerScriptAccess.isPlayersTurnLocal)
        {
            SetCardActivityStatus(true, 2);
        }        
        if (cardDebt > 0)
        {
            foreach(CardQueueUnit queUnit in cardQueDataList)
            {
                if(!isFullRefill)
                {
                    GenerateCard(new Vector3(0,0,0), CalculateCardIndex());
                    //GenerateCard(queUnit.QueuedVector, queUnit.QueuedIndex);
                }
                else
                {
                    GenerateCard(new Vector3(0, 0, 0), CalculateCardIndex());
                    //GenerateCard(queUnit.QueuedVector, queUnit.QueuedIndex);
                    if (cardCount >= cardLimit)
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

    public void SendCardsOver(Transform card)
    {
        if (canInteract && playerScriptAccess.isThisPlayersTurn)
        {
            if (card.GetComponentInParent<CardScript>().isClickable)
            {
                utlCardsPlayedForOtherPlayer++;
                int clickedCardId = card.GetComponentInParent<CardScript>().myCardId;
                playerScriptAccess.PlayCardForOtherPlayer(clickedCardId);
                deckManagerAccess.handCardList.Remove(clickedCardId);
                deckManagerAccess.discardedCardList.Add(clickedCardId);
                RebuildCardList(card.root.gameObject);
            }             
        }
    }
    public void HandReset()
    {
        foreach (CardScript card in cardList)
        {
            if (card != null)
            {
                Destroy(card.transform.root.gameObject);
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
        RebuildCardListLite();
        //canInteract = true;
        //isInQuickAttackMode = false;
    }

    public void DiscardCard(Transform card)
    {
        int cardId = card.GetComponentInParent<CardScript>().myCardId;       
        deckManagerAccess.handCardList.Remove(cardId);
        deckManagerAccess.discardedCardList.Add(cardId);
        RebuildCardList(card.root.gameObject);
    }

}
