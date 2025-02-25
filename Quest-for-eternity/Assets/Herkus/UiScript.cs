using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class UiScript : MonoBehaviour
{
    [SerializeField]
    static TextMeshPro fieldDamageText;

    [SerializeField]
    static TextMeshPro actionWindowText;

    [SerializeField]
    static TextMeshPro turnInfoText;

    [SerializeField]
    static TextMeshProUGUI[] uiTextArray;

    [SerializeField]
    public GameObject shuffleWindow;

    [SerializeField]
    public Button endTurnButton;

    [SerializeField]
    Image instanceImage;

    [SerializeField]
    Transform iconSpawnpoint;

    [SerializeField]
    DatabasePlayer databasePlayerAccess;

    public List<GameObject> iconList;


    private void Start()
    {
        instanceImage.gameObject.SetActive(false);
        uiTextArray = GetComponentsInChildren<TextMeshProUGUI>();
        UpdateTurnInfo(0);
        Invoke("UiManagerSubscription", 1f);
    }

    private void UiManagerSubscription()
    {
      RefereeScript.instance.restartGameEvent += DestroyAllIcons;
    }
    public void GenerateIcon(Sprite inputSprite)
    {
        GameObject newIcon = Instantiate(instanceImage.gameObject, iconSpawnpoint.position + new Vector3(0,-4,0), Quaternion.identity, transform.parent);
        newIcon.SetActive(true);
        newIcon.GetComponent<Image>().sprite = inputSprite;
        iconList.Add(newIcon);
        SquashIcons();
    }
    public void DestroyIcon(int cardId, int effectEntry)
    {
        //Debug.Log("destroying");
        Utility utilityAccess = databasePlayerAccess.cardList[cardId] as Utility;
        List<EffectUnit> uiEffectList = new List<EffectUnit>();
        if (utilityAccess)
        {
            uiEffectList = utilityAccess.effectUnitList;
        }
        else
        {
            Action actionAccess = databasePlayerAccess.cardList[cardId] as Action;
            uiEffectList = actionAccess.actionEffectUnitList;
            //Debug.Log("za giro");
        }
        EffectUnit myEffectUnit = uiEffectList[effectEntry];

        int additionalLoopCount = 0;
        if (myEffectUnit.effectIcon)
        {
            //Debug.Log("za giro 3");
            for (int i = 0; i < iconList.Count + additionalLoopCount; i++)
            {
                //Debug.Log($"ilgis: {iconList.Count}");
                int trueIndex;
                if(additionalLoopCount <= 0)
                {
                    trueIndex = i;
                }
                else
                {
                    trueIndex = i - additionalLoopCount;
                }
                //Debug.Log($"right side: {myEffectUnit.effectIcon.name}, left side: {iconList[trueIndex].GetComponent<Image>().sprite.name}");
                if (myEffectUnit.effectIcon == iconList[trueIndex].GetComponent<Image>().sprite)
                {
                    StartCoroutine(DestructionCoroutine(trueIndex));
                    GameObject tweenReference = iconList[trueIndex];
                    tweenReference.transform.DOMove(tweenReference.transform.position + new Vector3(0, -100, 0), 0.8f);
                    iconList[trueIndex] = null;
                    List<GameObject> newList = new List<GameObject>();

                    foreach (GameObject item in iconList)
                    {
                        if (item != null)
                        {
                            newList.Add(item);
                        }
                    }
                    iconList.Clear();
                    foreach (GameObject item in newList)
                    {
                        iconList.Add(item);
                    }
                    SquashIconsAfterClear();
                    //break;
                    additionalLoopCount++;
                }                   
            }
        }         
    }

    private void DestroyAllIcons()
    {
     /*   DestroyIcon(26, 0);
        DestroyIcon(26, 1);
        DestroyIcon(29, 0);
        DestroyIcon(10, 0);
        DestroyIcon(30, 0);
        DestroyIcon(27, 0);
        DestroyIcon(15, 0);
        DestroyIcon(22, 0); */
        foreach(GameObject gameObj in iconList)
        {
            Destroy(gameObj);
        }
        iconList.Clear();
    }
    private IEnumerator DestructionCoroutine(int inputId)
    {
        Debug.Log("coroutine");
        GameObject destructableReference = iconList[inputId];
        yield return new WaitForSeconds(1);
        Destroy(destructableReference);
    }
    private void SquashIcons()
    {
        float interval = 475f/(iconList.Count + 1);
        for (int i = 0; i < iconList.Count; i++)
        {
            if(i == iconList.Count - 1)
            {
                iconList[i].transform.position = new Vector3(iconSpawnpoint.position.x + interval * (i + 1), iconSpawnpoint.position.y - 250, iconSpawnpoint.position.z);
            }
            iconList[i].transform.DOMove(new Vector3(iconSpawnpoint.position.x + interval * (i + 1), iconSpawnpoint.position.y, iconSpawnpoint.position.z), 0.3f);
        }
    }

    private void SquashIconsAfterClear()
    {
        float interval = 475f / (iconList.Count + 1);
        for (int i = 0; i < iconList.Count; i++)
        {
            iconList[i].transform.DOMove(new Vector3(iconSpawnpoint.position.x + interval * (i + 1), iconSpawnpoint.position.y, iconSpawnpoint.position.z), 0.3f);
        }
    }
    public static void UpdateFighterText(TextMeshPro changedText, int value)
    {
        changedText.text = value.ToString();
    }

    public static void UpdateTurnInfo(int inputInfo)
    {
        string turnInfoString = "";

        if (inputInfo == 0)
        {
            turnInfoString = "your";
        }
        else
        {
            turnInfoString = "the enemy's";
        }
        uiTextArray[1].text = $"It is {turnInfoString} turn!";
    }

    public static void UpdateFieldDamageText(string inputString, bool isPlayerAttacking)
    {
        string attackerName;
        if(isPlayerAttacking)
        {
            attackerName = "The player";
        }
        else
        {
            attackerName = "The enemy";
        }
        uiTextArray[0].text = $"{attackerName}'s total damage: " + inputString;
    }
    
    public void ChangeEndTurnButtonStatus(bool inputBool)
    {
        //endTurnButton.interactable = inputBool;
    }
    public static void UpdateGameOverText(string inputString)
    {
        uiTextArray[2].text = inputString;
    }
    public void ToggleShuffleWindow(bool inputBool)
    {
        shuffleWindow.SetActive(inputBool);
    }
}
