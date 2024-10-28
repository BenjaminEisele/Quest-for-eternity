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
    private List<GameObject> cardList;

    [SerializeField]
    int cardCount = 0;

    Vector3 cardPlacementVector;

    void Start()
    {
        CardInstantiation();
    }

    void Update()
    {
        if(turnScriptAccess.isPlayersTurn)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //  Physics.Raycast(ray, out RaycastHit hitInfo)
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.transform.GetComponent<CardScript>())
                    {
                        fieldScriptAccess.SpawnActiveCard(hit.transform.GetComponent<CardScript>().cardId);
                        Destroy(hit.transform.gameObject);
                        cardCount--;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                AddCardsToHand(2);
            }
        }
    }

    public void CardInstantiation()
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
        //Refill count should be 0 if we want to 
        int refillCycleCount;
        bool isFullRefill;
        if(refillCount <= 0)
        {
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
                    /*   GameObject cardClone = Instantiate(baseCard, cardSpawnLocator.position + cardPlacementVector, Quaternion.identity);
                       cardClone.SetActive(true);
                       cardList[i] = cardClone;
                    */
                    GenerateCard(cardPlacementVector, i);
                    refillCount--;
                }

                cardPlacementVector += new Vector3(2, 0, 0);
            }
            if(refillCount > 0)
            {
                for (int i = 0; i < refillCount; i++)
                {
                    /*  GameObject cardClone = Instantiate(baseCard, cardSpawnLocator.position + cardPlacementVector, Quaternion.identity);
                      cardClone.SetActive(true);
                      cardList.Add(cardClone);*/
                    GenerateCard(cardPlacementVector, -1);
                    cardPlacementVector += new Vector3(2, 0, 0); 
                  
                }
            }
        }



       
    }
    void GenerateCard(Vector3 cardPlacementVectorReference, int cardIndex)
    {
        GameObject cardClone = Instantiate(baseCard, cardSpawnLocator.position + cardPlacementVectorReference, Quaternion.identity);
        cardClone.SetActive(true);
        cardCount++;
        if (cardIndex < 0)
        {
            cardList.Add(cardClone);
        }
        else
        {
            cardList[cardIndex] = cardClone;
        }
        
        
       
    }

}
