using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    public List<GameObject> iconList;


    private void Start()
    {
        instanceImage.gameObject.SetActive(false);
        uiTextArray = GetComponentsInChildren<TextMeshProUGUI>();
        UpdateTurnInfo(0);
    }

    public void GenerateIcon(Sprite inputSprite)
    {
        GameObject newIcon = Instantiate(instanceImage.gameObject, iconSpawnpoint.position, Quaternion.identity, transform.parent);
        newIcon.SetActive(true);
        newIcon.GetComponent<Image>().sprite = inputSprite;
        iconList.Add(newIcon);
        SquashIcons();
    }

    private void SquashIcons()
    {
        float interval = 475f/(iconList.Count + 1);
        for (int i = 0; i < iconList.Count; i++)
        {
            Debug.Log($"coordinate is {0 + interval * (i + 1)}");

            iconList[i].transform.position = new Vector3(iconSpawnpoint.position.x + interval * (i + 1), iconSpawnpoint.position.y, iconSpawnpoint.position.z);
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
        endTurnButton.interactable = inputBool;
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
