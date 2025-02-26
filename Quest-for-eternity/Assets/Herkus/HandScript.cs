using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    UiScript uiScriptAccess;

    [SerializeField]
    ChooseNewCardScript chooseNewCardScriptAccess;

    [SerializeField]
    int cardCount = 0;

    [SerializeField] SoundFXManager soundFXManager;
    [SerializeField] VoiceManager voiceManager;

    Vector3 cardPlacementVector;
    Coroutine handScriptDelayCoroutine;

    [HideInInspector]
    public bool canInteract;

    [HideInInspector]
    public bool isInQuickAttackMode;

	public bool isInLongShotMode;

	public bool isInMergeMode;

    [HideInInspector]
    public int utilityCount;
    public int utilityLimit;
    int cardDebt;
    public List<CardQueueUnit> cardQueDataList;
    int cardQueIndex;
    bool isHitrateAffected;
    
    [HideInInspector]
    public bool canPlayUtility;
    float savedHitrateDelta;

    //[HideInInspector]
    public int utlCardsPlayedForOtherPlayer;

    [SerializeField]
    GameObject damageSliderObject;

    bool isFullRefill;

    public bool isInDamageSliderMode;
    private int clickedCardId;

    public bool isHelpAndLeadActive;

    public TextMeshPro deckText;
    public GameObject deckCard;

    public GameObject discardPile;

    private void Start()
    {
        utilityLimit = 3;
        cardLimit = 5;
        Invoke("SubscriptionInvokeHand", 1f);
        turnScriptAccess.endTurnEvent += AddCardsEvent;
        turnScriptAccess.endTurnEvent += RebuildCardListLite;
        //turnScriptAccess.endTurnEvent += ResetQuickAttackMode;
        turnScriptAccess.endTurnEvent += HitRateRestoriationMethod;
        turnScriptAccess.endTurnEvent += ShowDiscardPile;
        //RefereeScript.instance.restartGameEvent += HandReset;
        //RefereeScript.instance.restartGameEvent += RebuildCardListLite;
		damageSliderObject.SetActive(false);
        isInQuickAttackMode = false;
        isInDamageSliderMode = false;
        isInMergeMode = false;
        isInLongShotMode = false;
        isHelpAndLeadActive = false;
        cardCount = 0;
        cardDebt = 0;
        cardQueIndex = 0;
        canInteract = true;
        deckManagerAccess.ShuffleCards(deckManagerAccess.deckCardList);
        CardInstantiation();
        ActivateAllCardsEvent();
        RebuildCardListLite();
        utlCardsPlayedForOtherPlayer = 0;
        deckText.text = deckManagerAccess.deckCardList.Count.ToString();
        discardPile.SetActive(false);
    }

    private void SetCanInteractTrue()
    {
        if(!chooseNewCardScriptAccess.isInLootingPhase)
        {
            canInteract = true;
        }
    }

    private void SubscriptionInvokeHand()
    {
        RefereeScript.instance.turnStartEvent += NewTurnHandLogic;
        RefereeScript.instance.turnStartEvent += SetCanInteractTrue;
        RefereeScript.instance.preNewWaveEvent += DisableAllCardsEvent;
        RefereeScript.instance.preNewWaveEvent += DeactivateDiscardPile;
        RefereeScript.instance.restartGameEvent += EffectAndVariableReset;
    }

    private void DeactivateDiscardPile()
    {
        discardPile.SetActive(false);
    }

    private void EffectAndVariableReset()
    {
        isInQuickAttackMode = false;
        isInDamageSliderMode = false;
        isInMergeMode = false;
        isInLongShotMode = false;
        isHelpAndLeadActive = false;
        canPlayUtility = true;
        utlCardsPlayedForOtherPlayer = 0;
        utilityLimit = 3;
        cardLimit = 5;
        damageSliderObject.SetActive(false);
        discardPile.SetActive(false);

        RestoreAllOriginalHitrates();
        StopAllCoroutines();
    }
    private void ShowDiscardPile()
    {
        if(deckManagerAccess.discardedCardList.Count > 0)
        {
            discardPile.SetActive(true);
        }
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
                    else if (fieldScriptAccess.SpawnActiveCard(clickedCardId, false, false))
                    {
                        canInteract = false;
                        if (isInQuickAttackMode)
                        {
                            handScriptDelayCoroutine = StartCoroutine(QuickAttackModeCoroutine(clickedCardId));
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
        float cardZLocator = -1f;
        for (int k = 0; k < cardList.Count; k++)
        {
            if (cardList[k] != null)
            {
                float myRotValue = 45 - interval * (k + 1);
                cardList[k].gameObject.transform.root.localEulerAngles = new Vector3(0, 0, myRotValue);
                cardZLocator += 0.1f;
                cardList[k].gameObject.GetComponent<DragDrop>().cardPosition = cardList[k].gameObject.transform.localPosition;
                cardList[k].transform.root.position = new Vector3(cardList[k].transform.root.position.x, cardList[k].transform.root.position.y, 0);
                cardList[k].GetComponent<OnHoverScript>().zLocator = cardZLocator;
                cardList[k].transform.root.position += new Vector3(0, 0, cardZLocator);
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
        float cardZLocator = -1f;
        for (int k = 0; k < cardList.Count; k++)
        {
            if(cardList[k] != null)
            {
                float myRotValue = 45 - interval * (k + 1);
                cardList[k].gameObject.transform.root.localEulerAngles = new Vector3(0, 0, myRotValue);
                cardZLocator += 0.1f;
                cardList[k].gameObject.GetComponent<DragDrop>().cardPosition = cardList[k].gameObject.transform.localPosition;
                cardList[k].transform.root.position = new Vector3(cardList[k].transform.root.position.x, cardList[k].transform.root.position.y, 0);
                cardList[k].GetComponent<OnHoverScript>().zLocator = cardZLocator;
                cardList[k].transform.root.position += new Vector3(0, 0, cardZLocator);
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
        if(!isHelpAndLeadActive)
        {
            foreach (CardScript card in cardList)
            {
                if (card != null)
                {
                    card.RestroreOriginalHitrate();
                }
            }
            ChangeAllVisualHitrates(true, 0, false);
        }       
    }
    public void ChangeAllVisualHitrates(bool shouldRestoreOriginal, float effectValue, bool shouldAddToValue)
    {
        isHitrateAffected = !shouldRestoreOriginal;
        savedHitrateDelta = effectValue;
        foreach (CardScript card in cardList)
        {
            if (card != null)
            {
                card.ChangeVisualCardHitrate(shouldRestoreOriginal, effectValue, shouldAddToValue);
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
        Invoke("AddCardsEventLogic", 0.5f);
    }

    private void AddCardsEventLogic()
    {
        AddCardsToHand(0);
        utilityCount = 0;
        RebuildCardListLite();
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
                uiScriptAccess.DestroyIcon(10, 0);
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
                //RestoreAllOriginalHitrates();
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

    private IEnumerator QuickAttackModeCoroutine(int inputCardId)
    {
        yield return new WaitForSeconds(0.75f);
        //ActionCardEffectActivation(inputCardId);
        playerScriptAccess.DealDamagePlayerScript(false, false, 0, false, true);
        isInQuickAttackMode = false;
        isHelpAndLeadActive = false;
        uiScriptAccess.DestroyIcon(30, 0);
        SetCardActivityStatus(true, 0);
        RestoreAllOriginalHitrates();
        canInteract = true;
        ActionCardEffectActivation(inputCardId);
    }
    private IEnumerator EndTurnDelayCoroutine(int inputCardId)
    {
        SetCardActivityStatus(false, 2);
        yield return new WaitForSeconds(0.75f);
        
        isHelpAndLeadActive = false;
        uiScriptAccess.DestroyIcon(30, 0);

        if (!isInDamageSliderMode)
        {
            turnScriptAccess.CallEndTurnEvent();
        }
        else
        {
            damageSliderObject.SetActive(true);
        }
        //ActionCardEffectActivation(inputCardId);
    }

    private IEnumerator MergedCoroutine(int firstId, int secondId)
    {
        SetCardActivityStatus(false, 2);
        yield return new WaitForSeconds(0.75f);
        isHelpAndLeadActive = false;
        uiScriptAccess.DestroyIcon(30, 0);

        ActionCardEffectActivation(firstId);
        ActionCardEffectActivation(secondId);
        turnScriptAccess.CallEndTurnEvent();
    }
    private void ActionCardEffectActivation(int inputCardId)
    {
        Action actionCardAccess = databasePlayerAccess.cardList[inputCardId] as Action;
        foreach (EffectUnit myEffectUnit in actionCardAccess.actionEffectUnitList)
        {
            if (!myEffectUnit.shouldActivateNow)
            {
                myEffectUnit.myEffect.UseEffect<GameObject>(RefereeScript.instance.chosenEnemyId, myEffectUnit.effectValue, sceneObjectAccess.gameObject);
            }
        }
    }

    public void DelayedActionCardEffectActivation()
    {
        if (clickedCardId != -1)
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
        if (deckManagerAccess.deckCardList.Count <= 0)
        {
            deckCard.SetActive(false);
        }
        if(isHitrateAffected)
        {
            cardClone.GetComponentInChildren<CardScript>().ChangeVisualCardHitrate(false, savedHitrateDelta, false);
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
            if(cardIndex + 1 > cardList.Count)
            {
                cardList.Add(cardClone.GetComponentInChildren<CardScript>());
            }
            else
            {
                cardList[cardIndex] = cardClone.GetComponentInChildren<CardScript>();
            }          
        }
        cardClone.GetComponentInChildren<CardScript>().SetCardActiveStatus(turnScriptAccess.isPlayersTurn);
        soundFXManager.DrawSound();
        deckText.text = deckManagerAccess.deckCardList.Count.ToString();
        if (!playerScriptAccess.isPlayersTurnLocal)
        {
            SetCardActivityStatus(false, 2);
        }
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
        return returnIndex;
    } 
    public void DrawQueuedCards()
    {
        if (playerScriptAccess.isPlayersTurnLocal)
        {
            SetCardActivityStatus(true, 2);
        }
        /*else
        {
            SetCardActivityStatus(false, 2);
        }*/
        if (cardDebt > 0)
        {
            foreach(CardQueueUnit queUnit in cardQueDataList)
            {
                if(!isFullRefill)
                {
                    GenerateCard(new Vector3(0,0,0), CalculateCardIndex());
                }
                else
                {
                    GenerateCard(new Vector3(0, 0, 0), CalculateCardIndex());
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

    public void SendCardsOver(Transform card, int customInput)
    {
        if (canInteract && playerScriptAccess.isThisPlayersTurn)
        {
            if (Random.Range(0, 100) < voiceManager.miscLineChance)
            {
                voiceManager.PlayCardForAlly();
            }

            int clickedCardId;
            if (customInput == -1)
            {
                if (card.GetComponentInParent<CardScript>().isClickable)
                {
                    utlCardsPlayedForOtherPlayer++;
                    clickedCardId = card.GetComponentInParent<CardScript>().myCardId;

                    playerScriptAccess.PlayCardForOtherPlayer(clickedCardId, true);
                    deckManagerAccess.handCardList.Remove(clickedCardId);
                    deckManagerAccess.discardedCardList.Add(clickedCardId);
                    RebuildCardList(card.root.gameObject);               
                }
            }
            else
            {
                clickedCardId = customInput;
                playerScriptAccess.PlayCardForOtherPlayer(clickedCardId, false);
            }
        }
        else if(customInput != -1)
        {
            clickedCardId = customInput;
            playerScriptAccess.PlayCardForOtherPlayer(clickedCardId, false);
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
        utlCardsPlayedForOtherPlayer = 0;
        CardInstantiation();
        RebuildCardListLite();
    }

    public void DiscardCard(Transform card)
    {
        int cardId = card.GetComponentInParent<CardScript>().myCardId;
        deckManagerAccess.handCardList.Remove(cardId);
        deckManagerAccess.discardedCardList.Add(cardId);
        RebuildCardList(card.root.gameObject);
        discardPile.SetActive(true);
    }

}
