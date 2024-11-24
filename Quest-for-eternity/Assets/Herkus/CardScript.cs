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

    Vector2 cardHitFraction;

    string myCardName;

    [SerializeField]
    GameObject myCardImage;


    [SerializeField]
    GameObject myCardOutline;

    [HideInInspector]
    public bool isClickable;


    //[HideInInspector]
    //public int cardType;
    //0 - utility
    //1 - action

    [HideInInspector]
    public bool isActionCard;

    private void Awake()
    {
        
    }
    
    public void HandCardSetup(int myId)
    {
        isClickable = true;
        //myCardId = Random.Range(0, databaseAccess.cardList.Count);
        myCardId = myId;
        Debug.Log($"My setup ID is {myId}");
        // MIGHT BE IMPORTANT
        //HealEffect generatedCardEffect = ScriptableObject.CreateInstance("HealEffect") as HealEffect;
        //BaseEffect generatedCardEffect = ScriptableObject.CreateInstance("BaseEffect") as BaseEffect;
        // generatedCardEffect.ExecuteEffect(5);
        //databaseAccess.cardDatabase[myCardId].cardEffect[0].ExecuteEffect(databaseAccess.cardDatabase[myCardId].cardEffect[0].damage);

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
        }


        GetComponentInChildren<SpriteRenderer>().color = myCardColor;
        myCardImage.GetComponent<SpriteRenderer>().sprite = databaseAccess.cardList[myCardId].cardSprite;
        cardTextArray = GetComponentsInChildren<TextMeshPro>();

        cardTextArray[0].text = myDamage.ToString();
        cardTextArray[1].text = cardTypeName;//CalculateString(cardType);
        cardTextArray[2].text = myCardName;
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
