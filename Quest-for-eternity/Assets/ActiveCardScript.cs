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

    public int ActiveCardSetup(int activeCardId)
    {
        activeCardDamage = databaseAccess.cardDatabase[activeCardId].damage;
        GetComponent<SpriteRenderer>().color = databaseAccess.cardDatabase[activeCardId].cardColor;

        activeCardText = GetComponentInChildren<TextMeshPro>();
        activeCardText.text = activeCardDamage.ToString();

        return activeCardDamage;
    }
}
