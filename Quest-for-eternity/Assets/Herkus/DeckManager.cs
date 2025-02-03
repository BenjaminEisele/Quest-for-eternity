using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DeckManager : MonoBehaviour
{
    public List<int> deckCardList;
    public List<int> handCardList;
    public List<int> discardedCardList;

    [SerializeField]
    UiScript uiScripAccess;

    [SerializeField]
    HandScript handScriptAccess;

    [SerializeField]
    PlayerScript playerScriptAccess;

    private void Start()
    {
        RefereeScript.newWaveEvent += ResetAllCardLists;
        TurnScript.restartGameEvent += ResetAllCardLists;
        uiScripAccess.ToggleShuffleWindow(false);
    }
    public void ResetDeckBegin()
    {
        uiScripAccess.ToggleShuffleWindow(true);
    }


    public void ResetDeck(bool shouldShuffle)
    {
        if(shouldShuffle)
        {
            ShuffleCards(discardedCardList);
        }
        deckCardList.AddRange(discardedCardList);
        discardedCardList.Clear();
        uiScripAccess.ToggleShuffleWindow(false);
        handScriptAccess.DrawQueuedCards();
        handScriptAccess.canInteract = true;
    }

    public void ResetAllCardLists()
    {
        deckCardList.AddRange(discardedCardList);
        discardedCardList.Clear();
        deckCardList.AddRange(handCardList);
        handCardList.Clear();
        ShuffleCards(deckCardList);
    }

    public void ShuffleCards(List<int> inputList)
    {
        int lenght = inputList.Count;
        int temp;

        int switchableA;
        int switchableB;
        if(inputList.Count > 1)
        {
            for (int i = 0; i < lenght; i++)
            {
                switchableA = 0;
                switchableB = 0;
                while (switchableA == switchableB)
                {
                    switchableA = Random.Range(0, lenght);
                    switchableB = Random.Range(0, lenght);
                }

                temp = inputList[switchableA];
                inputList[switchableA] = inputList[switchableB];
                inputList[switchableB] = temp;
            }
        }
    }
}
