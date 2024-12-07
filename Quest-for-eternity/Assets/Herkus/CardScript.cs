using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;


public class CardScript : MonoBehaviour
{
    private Color myCardColor;

    [SerializeField]
    Database databaseAccess;

    public TextMeshPro[] cardTextArray;
    public int myCardId;

    int myDamage;
    public float myCardHitRate;
    public float savedCardHitRate;

    Vector2 cardHitFraction;

    string myCardName;

    [SerializeField]
    GameObject myCardImage;


    [SerializeField]
    GameObject myCardOutline;

    [HideInInspector]
    public bool isClickable;

    string hitRateString;
    //[HideInInspector]
    //public int cardType;
    //0 - utility
    //1 - action

    [HideInInspector]
    public bool isActionCard;

    public void HandCardSetup(int myId)
    {
        isClickable = true;
        myCardId = myId;

        myCardColor = databaseAccess.cardList[myCardId].cardColor;
        myCardName = databaseAccess.cardList[myCardId].cardName;



        string cardTypeName;
        Utility utilityCardAccess = databaseAccess.cardList[myCardId] as Utility;
        if (utilityCardAccess)
        {
            //utilityCardAccess.effectList[0].UseEffect<string>(123, "asdf");
            myDamage = 0;
            cardTypeName = "Utility";
            isActionCard = false;
        }
        else
        {
            Action actionCardAccess = databaseAccess.cardList[myCardId] as Action;
            myDamage = actionCardAccess.cardDamage;
            cardTypeName = "Action";
            isActionCard = true;
            myCardHitRate = actionCardAccess.cardHitRate;
            savedCardHitRate = myCardHitRate;
        }



        

        GetComponentInChildren<SpriteRenderer>().color = myCardColor;
        myCardImage.GetComponent<SpriteRenderer>().sprite = databaseAccess.cardList[myCardId].cardSprite;


        cardTextArray = GetComponentsInChildren<TextMeshPro>();

        cardTextArray[0].text = myDamage.ToString();
        cardTextArray[1].text = cardTypeName;//CalculateString(cardType);
        cardTextArray[2].text = myCardName;
        cardTextArray[3].text = databaseAccess.cardList[myCardId].cardDescription;
        if(isActionCard)
        {
            hitRateString = myCardHitRate * 100 + " %";
            cardTextArray[4].text = hitRateString;
            cardTextArray[4].text = hitRateString;
        }
        else
        {
            cardTextArray[4].gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void RestroreOriginalHitrate()
    {
        myCardHitRate = savedCardHitRate;
        ChangeVisualCardHitrate(true, 0);
    }
    public void ChangeVisualCardHitrate(bool shouldRestoreOriginal, float hitRateChange)
    {
        if(isActionCard)
        {
            if (shouldRestoreOriginal)
            {
                myCardHitRate = savedCardHitRate;
                hitRateString = myCardHitRate * 100 + " %";
                cardTextArray[4].text = hitRateString;
            }
            else
            {
                myCardHitRate += hitRateChange;
                hitRateString = myCardHitRate * 100 + " %";
                cardTextArray[4].text = hitRateString;
            }
        }
    }
    public void SetCardActiveStatus(bool desiredStatus)
    {
        isClickable = desiredStatus;
        myCardOutline.SetActive(desiredStatus);
    }
    private string CalculateString(int cardTypeInput)
    {
        if(cardTypeInput == 0)
        {
            return "Utility";
        }
        else
        {
            return "Action";
        }
    }
}
