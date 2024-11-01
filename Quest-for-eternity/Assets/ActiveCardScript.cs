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

        GetComponentInChildren<SpriteRenderer>().color = databaseAccess.cardDatabase[activeCardId].cardColor;


        activeCardImage.GetComponent<SpriteRenderer>().sprite = databaseAccess.cardDatabase[activeCardId].cardSprite;
        activeCardType = databaseAccess.cardDatabase[activeCardId].cardType;
        activeCardDamage = databaseAccess.cardDatabase[activeCardId].damage;
        activeCardName = databaseAccess.cardDatabase[activeCardId].cardName;

        activeCardTextArray[0].text = activeCardDamage.ToString();
        activeCardTextArray[1].text = activeCardName;

        /* activeCardText = GetComponentInChildren<TextMeshPro>();
         activeCardText.text = activeCardDamage.ToString();
        */

        return activeCardDamage;
    }
}
