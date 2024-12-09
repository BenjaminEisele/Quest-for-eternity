using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChooseNewCardScript : MonoBehaviour
{
    public GameObject displayCardReferenceGameobject;
    public Transform displayCardLocator;
    public Database databaseAccess;
    public List<GameObject> displayCardList;
    [SerializeField]
    RefereeScript refereeScriptAccess;

    private void Start()
    {
       // DisplayCards();
    }
    public void DisplayCardsHidden(int inputId)
    {
        Debug.Log("hiding cards");
        foreach(GameObject displayCardObject in displayCardList)
        {
            Destroy(displayCardObject);
            Debug.Log(displayCardObject.name);
        }
        displayCardList.Clear();
        databaseAccess.gameObject.GetComponent<DeckManager>().discardedCardList.Add(inputId);
        refereeScriptAccess.StartNextWave(false);
    }

    
    public void DisplayCards()
    {
        Debug.Log("display activated");
        Vector3 newDisplayCardLocation = displayCardLocator.position;
        for (int i = 0; i < 3; i++)
        {
            int inputId = Random.Range(0, databaseAccess.cardList.Count);
            GameObject displayCard = Instantiate(displayCardReferenceGameobject, newDisplayCardLocation, Quaternion.identity, transform);
            displayCard.GetComponent<DisplayCardScript>().DisplayCardSetup(inputId);
            newDisplayCardLocation += new Vector3(2.5f, 0, 0);
            displayCardList.Add(displayCard);
        }
    }
}
