using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class DisplayCardScript : MonoBehaviour
{
    private Color myCardColor;
    public Database databaseAccess;
    public TextMeshPro[] cardTextArray;

    int myDamage;
    int myCardId;
    public float myCardHitRate;
    
    string hitRateString;
    string myCardName;

    [SerializeField]
    GameObject myCardImage;

    [HideInInspector]
    public bool isActionCard;

    
    void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Whatever you want it to do.
            //Debug.Log("CLICKED ON DISPLAY CARD");
            CallDisplayCardsHidden();
        }
    }

    private void CallDisplayCardsHidden()
    {
        transform.parent.GetComponent<ChooseNewCardScript>().DisplayCardsHidden(myCardId);
    }
    public void DisplayCardSetup(int myId)
    {
        myCardId = 9;//myId;

        myCardColor = databaseAccess.cardList[myCardId].cardColor;
        myCardName = databaseAccess.cardList[myCardId].cardName;



        string cardTypeName;
        Utility utilityCardAccess = databaseAccess.cardList[myCardId] as Utility;
        if (utilityCardAccess)
        {
            cardTypeName = "Utility";
            isActionCard = false;
        }
        else
        {
            Action actionCardAccess = databaseAccess.cardList[myCardId] as Action;
            cardTypeName = "Action";
            myDamage = actionCardAccess.cardDamage;
            isActionCard = true;
            myCardHitRate = actionCardAccess.cardHitRate;
            //savedCardHitRate = myCardHitRate;
        }

        GetComponentInChildren<SpriteRenderer>().color = myCardColor;
        myCardImage.GetComponent<SpriteRenderer>().sprite = databaseAccess.cardList[myCardId].cardSprite;


        cardTextArray = GetComponentsInChildren<TextMeshPro>();

        cardTextArray[0].text = myDamage.ToString();
        cardTextArray[1].text = cardTypeName;//CalculateString(cardType);
        cardTextArray[2].text = myCardName;
        cardTextArray[3].text = databaseAccess.cardList[myCardId].cardDescription;
        if (isActionCard)
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
}
