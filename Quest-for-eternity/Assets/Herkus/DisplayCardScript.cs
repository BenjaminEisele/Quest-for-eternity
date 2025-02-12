using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class DisplayCardScript : MonoBehaviour
{
    public DatabasePlayer databasePlayerAccess;
    public TextMeshPro[] cardTextArray;

    int myDamage;
    [HideInInspector]
    public int myCardId;
    private float myCardHitRate;
    
    string hitRateString;
    string myCardName;

    [SerializeField]
    GameObject myCardImage;

    private bool isActionCard;

    [HideInInspector]
    public PlayerScript playerScriptAccess;

    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerScriptAccess.isThisPlayersTurnToChoose)
        {
            transform.parent.GetComponent<ChooseNewCardScript>().ChooseOneCard(gameObject, myCardId);
            if(!RefereeScript.instance.singlePlayerMode)
            {
                transform.parent.GetComponent<ChooseNewCardScript>().playerScriptAccess.BeginDisplayCardSynchronization(myCardId);
            }         
        }
    }
    private void CallDisplayCardsHidden()
    {
        transform.parent.GetComponent<ChooseNewCardScript>().ChooseOneCard(gameObject, myCardId);
    }
    public void DisplayCardSetup(int myId)
    {
        myCardId = myId;
        myCardName = databasePlayerAccess.cardList[myCardId].cardName;
       
        Utility utilityCardAccess = databasePlayerAccess.cardList[myCardId] as Utility;
        if (utilityCardAccess)
        {
            isActionCard = false;
        }
        else
        {
            Action actionCardAccess = databasePlayerAccess.cardList[myCardId] as Action;
            myDamage = actionCardAccess.cardDamage;
            isActionCard = true;
            myCardHitRate = actionCardAccess.cardHitRate;
        }
        myCardImage.GetComponent<SpriteRenderer>().sprite = databasePlayerAccess.cardList[myCardId].cardSprite;
        cardTextArray = GetComponentsInChildren<TextMeshPro>();
        cardTextArray[0].text = myDamage.ToString();
        cardTextArray[1].text = myCardName;
        cardTextArray[2].text = databasePlayerAccess.cardList[myCardId].cardDescription;
        if (isActionCard)
        {
            hitRateString = myCardHitRate * 100 + " %";
            cardTextArray[3].text = hitRateString;
        }
        else
        {
            cardTextArray[3].gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
