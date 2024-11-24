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
    DeckManager deckManagerAccess;

    [SerializeField] // kodel sitas veikia tik su serializefield arba padarant list'a public?
    private List<CardScript> cardList;
    //private List<GameObject> cardList;

    [SerializeField]
    int cardCount = 0;

    Vector3 cardPlacementVector;
    Coroutine handScriptDelayCoroutine;

    public bool canInteract;

    public bool isInQuickAttackMode;

    int cardDebt;
    public List<CardQueueUnit> cardQueDataList;
    int cardQueIndex;

    private void Start()
    {
        isInQuickAttackMode = false;
        cardCount = 0;
        cardDebt = 0;
        cardQueIndex = 0;
        canInteract = true;
        CardInstantiation();
    }

    private void Update()
    {
        if(canInteract && turnScriptAccess.GetPlayerTurnBool())
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
                                    Debug.Log("end turn by card");
                                    
                                    handScriptDelayCoroutine = StartCoroutine(EndTurnDelayCoroutine());
                                    //turnScriptAccess.EndPlayersTurn();   
                                }
                            }
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
        fieldScriptAccess.FieldClearAndDealDamage(true);
        isInQuickAttackMode = false;
        SetCardActivityStatus(true, 0);
        canInteract = true;
    }
    private IEnumerator EndTurnDelayCoroutine()
    {
        yield return new WaitForSeconds(0.75f);
        AddCardsToHand(0);
        turnScriptAccess.EndPlayersTurn();
        fieldScriptAccess.FieldClearAndDealDamage(true);
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
        //Refill count should be 0 if we want to 
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


        cardPlacementVector = new Vector3(1, 0, 0);
        
        if(isFullRefill)
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                if (cardList[i] == null)
                {
                    if(cardCount < cardLimit)
                    {
                        GenerateCard(cardPlacementVector, i);
                    }
                    /* GameObject cardClone = Instantiate(baseCard, cardSpawnLocator.position + cardPlacementVector, Quaternion.identity);
                     cardClone.SetActive(true);
                     cardList[i] = cardClone;
                     cardCount++; */
                   
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
                        refillCount--;
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

                cardPlacementVector += new Vector3(2, 0, 0);
            }
            if(refillCount > 0)
            {
                for (int i = 0; i < refillCount; i++)
                {
                    cardPlacementVector += new Vector3(2, 0, 0);

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
                }
            }
        }
       // Debug.Log("EEEE");
    }
    private void GenerateCard(Vector3 cardPlacementVectorReference, int cardIndex)
    {
        GameObject cardClone = Instantiate(baseCard, cardSpawnLocator.position + cardPlacementVectorReference, Quaternion.identity);
        cardClone.SetActive(true);

        cardClone.GetComponent<CardScript>().HandCardSetup(deckManagerAccess.deckCardList[deckManagerAccess.deckCardList.Count - 1]);
        deckManagerAccess.handCardList.Add(deckManagerAccess.deckCardList[deckManagerAccess.deckCardList.Count - 1]);
        deckManagerAccess.deckCardList.RemoveAt(deckManagerAccess.deckCardList.Count - 1);

        if (deckManagerAccess.deckCardList.Count <= 0)
        {
            Debug.Log("out of cards!");


            //List<GameObject> listOfGameObjects = new List<GameObject>();
            //GameObject[] arrayOfGameObjects = listOfGameObjects.ToArray();
            canInteract = false;
            //SetCardActivityStatus(false, 2);
            deckManagerAccess.ResetDeckBegin();
            
        }
        else if(cardDebt > 0)
        {
            Debug.Log("card debt activated");
            while(cardDebt > 0)
            {
                cardDebt--;
               // AddCardsToHand(1);
            }
            
            
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
    }

    public void DrawQueuedCards()
    {
        if(cardDebt > 0)
        {
            Debug.Log("reached this!");
            foreach(CardQueueUnit queUnit in cardQueDataList)
            {
                GenerateCard(queUnit.QueuedVector, queUnit.QueuedIndex);
            }
            cardQueDataList.Clear();
        }
    }
    private void TransferCardToHand()
    {

    }
}
