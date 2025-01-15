using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class FieldScript : MonoBehaviour
{
    [SerializeField]
    Transform spawnpoint;
    [SerializeField]
    GameObject baseActiveCard;
    [SerializeField]
    UiScript uiScriptAccess;

    public PlayerScript playerScriptAccess;
    //[SerializeField]
    //RefereeScript refereeScriptAccess;

    //[HideInInspector]
    [SerializeField]
    public static int damagePoints = 0;

    private Vector3 activeCardSpawnPosition;

    [SerializeField] //kodel null refas pasidaro sita istrynus? alio??
    private List<GameObject> activeCardList;

    ActiveCardScript actionCardReference;

    [HideInInspector]
    public float hitRateModifier;

    private void Start()
    {
        TurnScript.endTurnEvent += FieldClearEventTrue;
        TurnScript.restartGameEvent += FieldClearEventFalse;
        hitRateModifier = 0;
        activeCardSpawnPosition = spawnpoint.position;
    }

    private void FieldClearEventTrue()
    {
        FieldClearAndDealDamage(true);
    }
    private void FieldClearEventFalse()
    {
        FieldClearAndDealDamage(true);
    }
    public bool SpawnActiveCard(int cardId)
    {
        GameObject activeCardInstance = Instantiate(baseActiveCard, activeCardSpawnPosition, Quaternion.identity);

        int gog = activeCardInstance.GetComponent<ActiveCardScript>().ActiveCardSetup(cardId);
        damagePoints += gog;

        if(activeCardInstance.GetComponent<ActiveCardScript>().shouldShowCard)
        {
            activeCardSpawnPosition += new Vector3(2, 0, 0);
            activeCardInstance.SetActive(true);
            activeCardList.Add(activeCardInstance);
        }
        UiScript.UpdateFieldDamageText(damagePoints.ToString(), true);

        bool isSpawningActionCard = activeCardInstance.GetComponent<ActiveCardScript>().CheckIfCardHasActionType();
        if(isSpawningActionCard)
        {
            actionCardReference = activeCardInstance.GetComponent<ActiveCardScript>();
        }

        return isSpawningActionCard;
    }

    public void FieldClearAndDealDamage(bool doWeDealDamage)
    {
        foreach(GameObject activeCardMember in activeCardList)
        {
            activeCardMember.GetComponent<ActiveCardScript>().ActivateMyEffect();
            Destroy(activeCardMember);
        }
        activeCardList.Clear();
        activeCardSpawnPosition = spawnpoint.position;
        if (doWeDealDamage)
        {
            if(actionCardReference != null)
            {
                if (actionCardReference.DidActiveCardHit(hitRateModifier))
                {
                    RefereeScript.instance.dealDamageToEnemy(damagePoints);
                    //playerScriptAccess.damageThisRound = damagePoints;
                }
                else
                {
                    Debug.Log("hit failed");
                }
            } 
        }
        hitRateModifier = 0;
        damagePoints = 0;
        UiScript.UpdateFieldDamageText(damagePoints.ToString(), true);
    }
}
