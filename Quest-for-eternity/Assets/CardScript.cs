using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardScript : MonoBehaviour
{
    [HideInInspector]
    public Color savedColor;

    [SerializeField]
    Database databaseAccess;

    public TextMeshPro cardText;

    public int cardId;

    int damage;

    private void Awake()
    {
        cardId = Random.Range(0, databaseAccess.cardDatabase.Count);
        
        savedColor = databaseAccess.cardDatabase[cardId].cardColor;
        damage = databaseAccess.cardDatabase[cardId].damage;
        cardText = GetComponentInChildren<TextMeshPro>();
        cardText.text = damage.ToString();
        GetComponent<SpriteRenderer>().color = savedColor; 
    }
}
