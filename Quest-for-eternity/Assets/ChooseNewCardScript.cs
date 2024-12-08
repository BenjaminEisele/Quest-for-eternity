using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChooseNewCardScript : MonoBehaviour
{
    public GameObject displayCardReferenceGameobject;
    public Transform displayCardLocator;
    public Database databaseAccess;
    public List<GameObject> displayCardList;

    private void Start()
    {
        DisplayCards();
    }
    public void DisplayCardsHidden()
    {
        foreach(GameObject displayCardObject in displayCardList)
        {
            Destroy(displayCardObject);
        }
        displayCardList.Clear();
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
