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
    [SerializeField]
    HandScript handscriptAccess;

    [SerializeField] SoundFXManager soundFXManager;
    [SerializeField] VoiceManager voiceManager;
    float rnd;

    public static int damagePoints = 0;
    public static int boostPoints = 0;

    public int damagePointsLiquid = 0;
    [HideInInspector]
    public float hitRateModifier;
    Vector3 activeCardSpawnPosition;

    public List<GameObject> activeCardList;
    public List<GameObject> ghostCardList;

    public List<int> mergeIdList;

    private void Start()
    {
        hitRateModifier = 0;
        activeCardSpawnPosition = spawnpoint.position;
        Invoke("FieldSubscription", 1f);
    }

    private void FieldSubscription()
    {
        RefereeScript.instance.restartGameEvent += ResetFieldScript;
    }

    public bool SpawnActiveCard(int cardId, bool isMergeSetup, bool fromAlly)
    {
        if (fromAlly)
        {
            if (Random.Range(0, 100) < voiceManager.miscLineChance)
            {
                voiceManager.RecieveCardFromAlly();
            }
        }

        soundFXManager.PlayCardSound();
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
            activeCardSpawnPosition += new Vector3(1.15f, 0, 0);
            activeCardInstance.SetActive(true);
            activeCardList.Add(activeCardInstance);
        }
        else
        {
            ghostCardList.Add(activeCardInstance);
            activeCardInstance.SetActive(false);
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
        if (mergeIdList.Count >= 2)
        {
            SpawnActiveCard(0, true, false);
            handscriptAccess.MergedCardExecution(mergeIdList[0], mergeIdList[1]);
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

    private void ResetFieldScript()
    {
        boostPoints = 0;
        damagePoints = 0;
        damagePointsLiquid = 0;
        hitRateModifier = 0;
        mergeIdList.Clear();
        RestartGameFieldClear();
    }
    public void FieldClear()
    {
        int foreachCount = 0;
        foreach (GameObject activeCardMember in activeCardList)
        {
            Destroy(activeCardMember);
            foreachCount++;
        }
        if(foreachCount > 0)
        {
            handscriptAccess.discardPile.SetActive(true);
        }
        foreach (GameObject ghostCard in ghostCardList)
		{
			Destroy(ghostCard);
		}
        activeCardList.Clear();
        ghostCardList.Clear();
        activeCardSpawnPosition = spawnpoint.position;
    }
    private void RestartGameFieldClear()
    {
        foreach (GameObject activeCardMember in activeCardList)
        {
            Destroy(activeCardMember);
        }
        foreach (GameObject ghostCard in ghostCardList)
        {
            Destroy(ghostCard);
        }
        activeCardList.Clear();
        ghostCardList.Clear();
        activeCardSpawnPosition = spawnpoint.position;
    }

    public int CheckIfHitAndShouldClearField(bool inputBool, bool shouldGuaranteeHit)
    {
        bool didWeHit;

        if (playerScriptAccess.isThisPlayersTurn)
        {
            Debug.Log(damagePoints);
            Debug.Log(boostPoints);
            damagePointsLiquid = damagePoints + boostPoints;
            FieldEffectActivation();
            if (actionCardReference != null)
            {
              
                if (!shouldGuaranteeHit)
                {
                    didWeHit = actionCardReference.DidActiveCardHit(hitRateModifier);
                }
                else
                {
                    didWeHit = true;
                }
                
                if (didWeHit)
                {
                    soundFXManager.HitSound();
                    hitRateModifier = 0;
                    damagePoints = 0;
					boostPoints = 0;
                    UiScript.UpdateFieldDamageText(damagePoints.ToString(), true);
                    if(inputBool)
                    {
                        FieldClear();
                    }
                    return 0;
                }
                else
                {
                    if (Random.Range(0, 100) < voiceManager.miscLineChance)
                    {
                        voiceManager.MissedAttackLine();
                    }
                    soundFXManager.MissSound();
                    hitRateModifier = 0;
                    damagePoints = 0;
					boostPoints = 0;
                    UiScript.UpdateFieldDamageText(damagePoints.ToString(), true);
                    if (inputBool)
                    {
                        FieldClear();
                    }
                    return 1;

                }
            }
            else
            {
                hitRateModifier = 0;
                damagePoints = 0;
				boostPoints = 0;
                UiScript.UpdateFieldDamageText(damagePoints.ToString(), true);
                if (inputBool)
                {
                    FieldClear();
                }

                //return false;
                return 2;
            }
        }
        return 2;
    }
}