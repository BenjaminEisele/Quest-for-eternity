using UnityEngine;
using TMPro;
using DG.Tweening;


public class CardScript : MonoBehaviour
{ 
    public int myCardId;
    int myDamage;
    private float myCardHitRate;
    private float savedCardHitRate;

    string myCardName;
    string hitRateString;

    [SerializeField]
    GameObject myCardImage;
    [SerializeField]
    GameObject myCardOutline;
    [SerializeField]
    DatabasePlayer databasePlayerAccess;


    [HideInInspector]
    public bool isClickable;
    [HideInInspector]
    public bool isActionCard;


    private TextMeshPro[] cardTextArray;

    public void HandCardSetup(int myId)
    {
        isClickable = true;
        myCardId = myId;
        myCardName = databasePlayerAccess.cardList[myCardId].cardName;     
        string cardTypeName;
        Utility utilityCardAccess = databasePlayerAccess.cardList[myCardId] as Utility;
        if (utilityCardAccess)
        {
            myDamage = 0;
            cardTypeName = "Utility";
            isActionCard = false;
        }
        else
        {
            Action actionCardAccess = databasePlayerAccess.cardList[myCardId] as Action;
            myDamage = actionCardAccess.cardDamage;
            cardTypeName = "Action";
            isActionCard = true;
            myCardHitRate = actionCardAccess.cardHitRate;
            savedCardHitRate = myCardHitRate;
        }
        myCardImage.GetComponent<SpriteRenderer>().sprite = databasePlayerAccess.cardList[myCardId].cardSprite;
        cardTextArray = GetComponentsInChildren<TextMeshPro>();
        cardTextArray[0].text = myDamage.ToString();
        cardTextArray[1].text = cardTypeName;
        cardTextArray[2].text = myCardName;
        cardTextArray[3].text = databasePlayerAccess.cardList[myCardId].cardDescription;
        if(isActionCard)
        {
            hitRateString = myCardHitRate * 100 + "%";
            cardTextArray[4].text = hitRateString;
            cardTextArray[4].text = hitRateString;
        }
        else
        {
            cardTextArray[4].gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        transform.name = myCardName;
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
                hitRateString = myCardHitRate * 100 + "%";
                cardTextArray[4].text = hitRateString;
            }
            else
            {
                myCardHitRate += hitRateChange;
                hitRateString = Mathf.Round(myCardHitRate * 100) + "%";
                cardTextArray[4].text = hitRateString;
            }
        }
    }
    public void SetCardActiveStatus(bool desiredStatus)
    {   
        isClickable = desiredStatus;
        myCardOutline.SetActive(desiredStatus);   
    }
}
