using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DeckManager : MonoBehaviour
{
    public List<int> deckCardList;
    public List<int> handCardList;
    public List<int> discardedCardList;
    public List<int> starterCardList;

    [SerializeField]
    UiScript uiScripAccess;

    [SerializeField]
    HandScript handScriptAccess;

    [SerializeField]
    PlayerScript playerScriptAccess;

    [SerializeField]
    TurnScript turnScriptAccess;

    [SerializeField] SoundFXManager soundFXManager;

    private void Start()
    {
        Invoke("SubscriptionInvokeDeck", 1f);
        uiScripAccess.ToggleShuffleWindow(false);
    }

    private void SubscriptionInvokeDeck()
    {
        RefereeScript.instance.newWaveEvent += ResetAllCardLists;
        RefereeScript.instance.restartGameEvent += ResetToStarterCards;
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
            soundFXManager.ShuffleSound();
        }
        else
        {
            soundFXManager.FlipSound();
        }
        deckCardList.AddRange(discardedCardList);
        discardedCardList.Clear();
        uiScripAccess.ToggleShuffleWindow(false);
        handScriptAccess.DrawQueuedCards();
        handScriptAccess.canInteract = true;
        handScriptAccess.deckText.text = deckCardList.Count.ToString();
        handScriptAccess.deckCard.SetActive(true);
        handScriptAccess.discardPile.SetActive(false);
    }

    public void ResetAllCardLists()
    {  
        if(playerScriptAccess.isLocalGamePlayer)
        {
            deckCardList.AddRange(discardedCardList);
            discardedCardList.Clear();
            deckCardList.AddRange(handCardList);
            handCardList.Clear();
            ShuffleCards(deckCardList);
            handScriptAccess.HandReset();
        } 
    }
    public void ResetToStarterCards()
    {
        if (playerScriptAccess.isLocalGamePlayer)
        {
            discardedCardList.Clear();
            handCardList.Clear();
            deckCardList.Clear();
            deckCardList.AddRange(starterCardList);
            ShuffleCards(deckCardList);
            handScriptAccess.HandReset();
            handScriptAccess.RebuildCardListLite();
        }
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
