using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChooseNewCardScript : MonoBehaviour
{
    public GameObject displayCardReferenceGameobject;
    public Transform displayCardLocator;
    public DatabasePlayer databasePlayerAccess;
    public List<GameObject> displayCardList;
    //[SerializeField]
    //RefereeScript refereeScriptAccess;

    public static ChooseNewCardScript instance;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

    private void Start()
    {
        // DisplayCards();
        RefereeScript.preNewWaveEvent += DisplayCards;
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
        databasePlayerAccess.gameObject.GetComponent<DeckManager>().discardedCardList.Add(inputId);
        //refereeScriptAccess.canTransferTurnToPlayer = true;
<<<<<<< HEAD
        refereeScriptAccess.CallStartTurnEvent();
        refereeScriptAccess.StartNextWave(false);
=======
        RefereeScript.instance.CallStartTurnEvent();
        RefereeScript.instance.StartNextWave(false);

>>>>>>> Multiplayer
    }

    
    public void DisplayCards()
    {
        Debug.Log("display activated");
        Vector3 newDisplayCardLocation = displayCardLocator.position;
        for (int i = 0; i < 3; i++)
        {
            int inputId = Random.Range(0, databasePlayerAccess.cardList.Count);
            GameObject displayCard = Instantiate(displayCardReferenceGameobject, newDisplayCardLocation, Quaternion.identity, transform);
            displayCard.GetComponent<DisplayCardScript>().DisplayCardSetup(inputId);
            newDisplayCardLocation += new Vector3(2.5f, 0, 0);
            displayCardList.Add(displayCard);
        }
    }
}
