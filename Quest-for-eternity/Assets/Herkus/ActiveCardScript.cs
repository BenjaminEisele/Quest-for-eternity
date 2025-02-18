using UnityEngine;
using TMPro;

public class ActiveCardScript : MonoBehaviour
{
    int activeCardDamage;
    private int activeCardId;
    float activeCardHitRate;
    private string activeCardName;

    bool isActionCard;
    bool alreadyActivated;
    [HideInInspector]
    public bool shouldShowCard;

    private TextMeshPro[] activeCardTextArray;

    [SerializeField]
    SceneObjectDatabase sceneObjectAccess;
    [SerializeField]
    DatabasePlayer databasePlayerAccess;
    [SerializeField]
    GameObject activeCardImage;
    public bool CheckIfCardHasActionType()
    {
        return isActionCard;
    }

    public bool DidActiveCardHit(float hitRateModifier)
    {
        float successChance = (activeCardHitRate + hitRateModifier) * 20;
        int successChanceInteger = (int)successChance;
        if(successChanceInteger <= 0)
        {
            return false;
        }
        else
        {
            int diceRoll = Random.Range(1, 21);
            Debug.Log($"Dice roll was: {diceRoll}, success chance was: {successChanceInteger}");
            if (diceRoll <= successChanceInteger)
            {             
                return true;                
            }
            else
            {
                return false;
            }
        }
       
    }

    public void ActivateMyEffect()
    {
        if (!isActionCard)
        {
            Utility utilityCardAccess = databasePlayerAccess.cardList[activeCardId] as Utility;
            if (utilityCardAccess)
            {
                foreach(EffectUnit myEffectUnit in utilityCardAccess.effectUnitList)
                {
                    if (!myEffectUnit.shouldActivateNow)
                    {
                        myEffectUnit.myEffect.UseEffect<GameObject>(RefereeScript.instance.chosenEnemyId, myEffectUnit.effectValue, sceneObjectAccess.gameObject);
                    }      
                }
            }
        } 
    }
    public int ActiveCardSetup(int activeCardId)
    {
        shouldShowCard = true;
        this.activeCardId = activeCardId;
        activeCardTextArray = GetComponentsInChildren<TextMeshPro>();
        Action actionCardAccess = databasePlayerAccess.cardList[activeCardId] as Action;
        if (actionCardAccess)
        {
            activeCardDamage = actionCardAccess.cardDamage;
            activeCardHitRate = actionCardAccess.cardHitRate;
            isActionCard = true;          
        }
        else
        {
            activeCardDamage = 0;
            isActionCard = false;

            Utility utilityCardAccess = databasePlayerAccess.cardList[activeCardId] as Utility;
            shouldShowCard = utilityCardAccess.isDisplayable;
            foreach (EffectUnit myEffectUnit in utilityCardAccess.effectUnitList)
            {
                if(myEffectUnit.shouldActivateNow)
                {
                    myEffectUnit.myEffect.UseEffect<GameObject>(RefereeScript.instance.chosenEnemyId, myEffectUnit.effectValue, sceneObjectAccess.gameObject);
                }
            }
        }
        activeCardImage.GetComponent<SpriteRenderer>().sprite = databasePlayerAccess.cardList[activeCardId].cardSprite;
        activeCardName = databasePlayerAccess.cardList[activeCardId].cardName;
        activeCardTextArray[0].text = activeCardDamage.ToString();
        activeCardTextArray[1].text = activeCardName;
        return activeCardDamage;
    }
    public int ActiveCardSetupMerged(int firstId, int secondId)
    {
        shouldShowCard = true;
        this.activeCardId = firstId;
        activeCardTextArray = GetComponentsInChildren<TextMeshPro>();
        Action firstActionCardAccess = databasePlayerAccess.cardList[firstId] as Action;
        Action secondActionCardAccess = databasePlayerAccess.cardList[secondId] as Action;

        if (firstActionCardAccess)
        {
            activeCardDamage = firstActionCardAccess.cardDamage + secondActionCardAccess.cardDamage;
            if(firstActionCardAccess.cardHitRate >= secondActionCardAccess.cardHitRate)
            {
                activeCardHitRate = firstActionCardAccess.cardHitRate;
            }
            else
            {
                activeCardHitRate = secondActionCardAccess.cardHitRate;
            }
            isActionCard = true;
        }
        else
        {
            activeCardDamage = 0;
            isActionCard = false;

            Utility utilityCardAccess = databasePlayerAccess.cardList[activeCardId] as Utility;
            shouldShowCard = utilityCardAccess.isDisplayable;
            foreach (EffectUnit myEffectUnit in utilityCardAccess.effectUnitList)
            {
                if (myEffectUnit.shouldActivateNow)
                {
                    myEffectUnit.myEffect.UseEffect<GameObject>(RefereeScript.instance.chosenEnemyId, myEffectUnit.effectValue, sceneObjectAccess.gameObject);
                }
            }
        }
        activeCardImage.GetComponent<SpriteRenderer>().sprite = databasePlayerAccess.cardList[activeCardId].cardSprite;
        activeCardName = databasePlayerAccess.cardList[activeCardId].cardName;
        activeCardTextArray[0].text = activeCardDamage.ToString();
        activeCardTextArray[1].text = activeCardName;
        return activeCardDamage;
    }
}
