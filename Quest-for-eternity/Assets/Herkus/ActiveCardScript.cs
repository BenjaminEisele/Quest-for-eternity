using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveCardScript : MonoBehaviour
{
    //Color activeCardColor;
    int activeCardDamage;

    [SerializeField]
    Database databaseAccess;

    TextMeshPro activeCardText;
    public TextMeshPro[] activeCardTextArray;

    private string activeCardName;

    private int activeCardType;
    private int activeCardId;

    bool isActionCard;
    bool isAlreadyActivated;

    [HideInInspector]
    public bool shouldShowCard;

    float activeCardHitRate;

    [SerializeField]
    SceneObjectDatabase sceneObjectAccess;

    [SerializeField]
    GameObject activeCardImage;


    public bool CheckIfCardHasActionType()
    {
        return isActionCard;
    }

    public bool DidActiveCardHit()
    {
        float successChance = activeCardHitRate * 20;
        int successChanceInteger = (int)successChance;
        if(Random.Range(1, 21) <= successChanceInteger)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ActivateMyEffect()
    {
        if (!isActionCard)
        {
            Utility utilityCardAccess = databaseAccess.cardList[activeCardId] as Utility;
            if (utilityCardAccess)
            {
                foreach(EffectUnit myEffectUnit in utilityCardAccess.effectUnitList)
                {
                    if (!myEffectUnit.shouldActivateNow)
                    {
                        myEffectUnit.myEffect.UseEffect<GameObject>(0, myEffectUnit.effectValue, sceneObjectAccess.gameObject);
                    }      
                }
                // utilityCardAccess.effectUnitList[0].myEffect.UseEffect<string>(utilityCardAccess.effectUnitList[0].effectValue, "asdf");
            }
        } 
    }
    public int ActiveCardSetup(int activeCardId)
    {
        shouldShowCard = true;
        isAlreadyActivated = false;
        this.activeCardId = activeCardId;
        activeCardTextArray = GetComponentsInChildren<TextMeshPro>();
       


        Action actionCardAccess = databaseAccess.cardList[activeCardId] as Action;
        if (actionCardAccess)
        {
            activeCardDamage = actionCardAccess.cardDamage;
            activeCardHitRate = actionCardAccess.cardHitRate;
            isActionCard = true;
            
        }
        else
        {
            activeCardDamage = 0;
            isActionCard = false;

            Utility utilityCardAccess = databaseAccess.cardList[activeCardId] as Utility;
            shouldShowCard = utilityCardAccess.isDisplayable;
            foreach (EffectUnit myEffectUnit in utilityCardAccess.effectUnitList)
            {
                if(myEffectUnit.shouldActivateNow)
                {
                    myEffectUnit.myEffect.UseEffect<GameObject>(0, myEffectUnit.effectValue, sceneObjectAccess.gameObject);
                }
            }
        }

        GetComponentInChildren<SpriteRenderer>().color = databaseAccess.cardList[activeCardId].cardColor;
        activeCardImage.GetComponent<SpriteRenderer>().sprite = databaseAccess.cardList[activeCardId].cardSprite;
        activeCardName = databaseAccess.cardList[activeCardId].cardName;

        activeCardTextArray[0].text = activeCardDamage.ToString();
        activeCardTextArray[1].text = activeCardName;

        /* activeCardText = GetComponentInChildren<TextMeshPro>();
         activeCardText.text = activeCardDamage.ToString();
        */

        return activeCardDamage;
    }
}
