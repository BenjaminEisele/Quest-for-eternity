using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;


public class CardScript : MonoBehaviour
{
    //[HideInInspector]
    private Color myCardColor;

    [SerializeField]
    Database databaseAccess;

   // public TextMeshPro myCardText;

    public TextMeshPro[] cardTextArray;
    public int myCardId;

    int myDamage;

    Vector2 cardHitFraction;

    string myCardName;

    [SerializeField]
    GameObject myCardImage;

    [HideInInspector]
    public int cardType;
    //0 - utility
    //1 - action
    
     

    private void Awake()
    {
        myCardId = Random.Range(0, databaseAccess.cardDatabase.Count);

        cardType = databaseAccess.cardDatabase[myCardId].cardType;

        cardHitFraction = databaseAccess.cardDatabase[myCardId].hitFraction;

        myCardColor = databaseAccess.cardDatabase[myCardId].cardColor;
        myDamage = databaseAccess.cardDatabase[myCardId].damage;
        myCardName = databaseAccess.cardDatabase[myCardId].cardName;

        databaseAccess.cardDatabase[myCardId].cardEffect[0].ExecuteEffect(databaseAccess.cardDatabase[myCardId].cardEffect[0].damage);
        
        // MIGHT BE IMPORTANT
        //HealEffect generatedCardEffect = ScriptableObject.CreateInstance("HealEffect") as HealEffect;
        //BaseEffect generatedCardEffect = ScriptableObject.CreateInstance("BaseEffect") as BaseEffect;
        // generatedCardEffect.ExecuteEffect(5);

        GetComponentInChildren<SpriteRenderer>().color = myCardColor;
        myCardImage.GetComponent<SpriteRenderer>().sprite = databaseAccess.cardDatabase[myCardId].cardSprite;


        cardTextArray = GetComponentsInChildren<TextMeshPro>();

        cardTextArray[0].text = myDamage.ToString();

        cardTextArray[1].text = CalculateString(cardType);

        cardTextArray[2].text = myCardName;
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
