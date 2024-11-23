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

    [SerializeField] // kodel sitas veikia tik su serializefield arba padarant list'a public?
    private List<CardScript> cardList;
    //private List<GameObject> cardList;

    [SerializeField]
    int cardCount = 0;

    Vector3 cardPlacementVector;
    Coroutine handScriptDelayCoroutine;

    public bool canInteract;

    public bool isInQuickAttackMode;

    private void Start()
    {
        isInQuickAttackMode = false;
        cardCount = 0;
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
                           
                            if (fieldScriptAccess.SpawnActiveCard(hit.transform.GetComponentInParent<CardScript>().myCardId))
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

    public void SetUtilityCardStatus(bool desiredCardStatus)
    {
        foreach (CardScript card in cardList)
        {
            if(card != null)
            {
                if (!card.isActionCard)
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
        SetUtilityCardStatus(true);
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
                    GenerateCard(cardPlacementVector, i);
                    refillCount--;
                }

                cardPlacementVector += new Vector3(2, 0, 0);
            }
            if(refillCount > 0)
            {
                for (int i = 0; i < refillCount; i++)
                {
                    GenerateCard(cardPlacementVector, -1);
                    cardPlacementVector += new Vector3(2, 0, 0); 
                  
                }
            }
        }
    }
    private void GenerateCard(Vector3 cardPlacementVectorReference, int cardIndex)
    {
        GameObject cardClone = Instantiate(baseCard, cardSpawnLocator.position + cardPlacementVectorReference, Quaternion.identity);
        cardClone.SetActive(true);
        
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
}
