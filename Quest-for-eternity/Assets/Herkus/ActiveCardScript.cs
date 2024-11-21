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

    [SerializeField]
    GameObject activeCardImage;

    public int GetActiveCardType()
    {
        return activeCardType;
    }
    public int ActiveCardSetup(int activeCardId)
    {
        activeCardTextArray = GetComponentsInChildren<TextMeshPro>();

        Action utilityCardAccess = databaseAccess.cardList[activeCardId] as Action;
        if (utilityCardAccess)
        {
            activeCardDamage = utilityCardAccess.cardDamage;
        }
        else
        {
            activeCardDamage = 0;
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
