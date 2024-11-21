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


    [SerializeField]
    GameObject activeCardImage;

    public bool CheckIfCardHasActionType()
    {
        return isActionCard;
    }

    public void ActivateMyEffect()
    {
        if(!isActionCard)
        {
            Utility utilityCardAccess = databaseAccess.cardList[activeCardId] as Utility;
            if (utilityCardAccess)
            {
                foreach(EffectUnit myEffectUnit in utilityCardAccess.effectUnitList)
                {
                    myEffectUnit.myEffect.UseEffect<string>(myEffectUnit.effectValue, "asdf");
                }
                // utilityCardAccess.effectUnitList[0].myEffect.UseEffect<string>(utilityCardAccess.effectUnitList[0].effectValue, "asdf");
            }
        } 
    }
    public int ActiveCardSetup(int activeCardId)
    {
        this.activeCardId = activeCardId;
        activeCardTextArray = GetComponentsInChildren<TextMeshPro>();

        Action actionCardAccess = databaseAccess.cardList[activeCardId] as Action;
        if (actionCardAccess)
        {
            activeCardDamage = actionCardAccess.cardDamage;
            isActionCard = true;
        }
        else
        {
            activeCardDamage = 0;
            isActionCard = false;
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
