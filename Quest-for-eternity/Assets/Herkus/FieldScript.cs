using UnityEngine;
using System.Collections.Generic;

public class FieldScript : MonoBehaviour
{
    [SerializeField]
    Transform spawnpoint;
    [SerializeField]
    GameObject baseActiveCard;
    [SerializeField]
    PlayerScript playerScriptAccess;
    ActiveCardScript actionCardReference;

    public static int damagePoints = 0;
    public int damagePointsLiquid = 0;
    [HideInInspector]
    public float hitRateModifier;
    Vector3 activeCardSpawnPosition;

    public List<GameObject> activeCardList;
    public List<int> mergeIdList;
   
    private void Start()
    {
        playerScriptAccess.shouldDealDamage = false;
        hitRateModifier = 0;
        activeCardSpawnPosition = spawnpoint.position;
    }

    public bool SpawnActiveCard(int cardId, bool isMergeSetup)
    {  
        GameObject activeCardInstance = Instantiate(baseActiveCard, activeCardSpawnPosition, Quaternion.identity);
        int damagePointsFromActiveCard;
        if (isMergeSetup)
        {
            damagePointsFromActiveCard = activeCardInstance.GetComponent<ActiveCardScript>().ActiveCardSetupMerged(mergeIdList[0], mergeIdList[1]);
        }
        else
        {
            damagePointsFromActiveCard = activeCardInstance.GetComponent<ActiveCardScript>().ActiveCardSetup(cardId);
        }
        
        damagePoints += damagePointsFromActiveCard;
        if (activeCardInstance.GetComponent<ActiveCardScript>().shouldShowCard)
        {
            activeCardSpawnPosition += new Vector3(2, 0, 0);
            activeCardInstance.SetActive(true);
            activeCardList.Add(activeCardInstance);
        }
        UiScript.UpdateFieldDamageText(damagePoints.ToString(), true);
        bool isSpawningActionCard = activeCardInstance.GetComponent<ActiveCardScript>().CheckIfCardHasActionType();
        if (isSpawningActionCard)
        {
            actionCardReference = activeCardInstance.GetComponent<ActiveCardScript>();
        }
        else if (activeCardInstance.GetComponent<ActiveCardScript>().shouldShowCard)
        {
            transform.root.GetComponentInChildren<HandScript>().utilityCount++;
        }
        return isSpawningActionCard;
    }
    public void InputCardForMerging(int inputCardId)
    {
        mergeIdList.Add(inputCardId);
        if(mergeIdList.Count >= 2)
        {
            SpawnActiveCard(0, true);
            mergeIdList.Clear();
        }
        
    }
    private void FieldEffectActivation()
    {
        foreach (GameObject activeCardMember in activeCardList)
        {
            activeCardMember.GetComponent<ActiveCardScript>().ActivateMyEffect();
        }
    }

    public void FieldClear()
    {
        foreach (GameObject activeCardMember in activeCardList)
        {
            Destroy(activeCardMember);
        }
        activeCardList.Clear();
        activeCardSpawnPosition = spawnpoint.position;
    }
    public bool CheckIfHitAndShouldClearField(bool inputBool, bool shouldGuaranteeHit)
    {
        bool didWeHit;

        if (playerScriptAccess.isThisPlayersTurn)
        {
            damagePointsLiquid = damagePoints;
            FieldEffectActivation();
            if (actionCardReference != null)
            {
                if(!shouldGuaranteeHit)
                {
                    didWeHit = actionCardReference.DidActiveCardHit(hitRateModifier);
                }
                else
                {
                    didWeHit = true;
                }
                
                playerScriptAccess.shouldDealDamage = didWeHit;
                if (didWeHit)
                {
                    hitRateModifier = 0;
                    damagePoints = 0;
                    UiScript.UpdateFieldDamageText(damagePoints.ToString(), true);
                    if(inputBool)
                    {
                        FieldClear();
                    }
                    return didWeHit;
                }
                else
                {
                    hitRateModifier = 0;
                    damagePoints = 0;
                    UiScript.UpdateFieldDamageText(damagePoints.ToString(), true);
                    if (inputBool)
                    {
                        FieldClear();
                    }
                    return false;

                }
            }
            else
            {
                hitRateModifier = 0;
                damagePoints = 0;
                UiScript.UpdateFieldDamageText(damagePoints.ToString(), true);
                if (inputBool)
                {
                    FieldClear();
                }

                return false;
            }
        }
        return false;
    }
}