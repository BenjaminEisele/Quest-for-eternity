using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class FieldScript : MonoBehaviour
{
    [SerializeField]
    Transform spawnpoint;
    [SerializeField]
    GameObject baseActiveCard;
    [SerializeField]
    UiScript uiScriptAccess;
    [SerializeField]
    RefereeScript refereeScriptAccess;

    TextMeshPro fieldDamageText;

    int damagePoints = 0;

    private Vector3 activeCardSpawnPosition;

    [SerializeField] //kodel null refas pasidaro sita istrynus? alio??
    private List<GameObject> activeCardList;

    


    private void Start()
    {
        activeCardSpawnPosition = spawnpoint.position;
        fieldDamageText = GetComponentInChildren<TextMeshPro>();
    }
    public void SpawnActiveCard(int cardId)
    {
       
        GameObject activeCardInstance = Instantiate(baseActiveCard, activeCardSpawnPosition, Quaternion.identity);
        activeCardSpawnPosition += new Vector3(2, 0, 0);
 
        activeCardList.Add(activeCardInstance);

        damagePoints += activeCardInstance.GetComponent<ActiveCardScript>().ActiveCardSetup(cardId);
        UpdateFieldDamageText();
    }

    public void FieldClearAndDealDamage(bool doWeDealDamage)
    {
        foreach(GameObject activeCardMember in activeCardList)
        {
            Destroy(activeCardMember);
        }
        activeCardList.Clear();
        activeCardSpawnPosition = spawnpoint.position;
        if (doWeDealDamage)
        {
            refereeScriptAccess.dealDamageToEnemy(damagePoints);
        }
        damagePoints = 0;
        UpdateFieldDamageText();
    }

    private void UpdateFieldDamageText()
    {
        fieldDamageText.text = damagePoints.ToString();
    }

}
