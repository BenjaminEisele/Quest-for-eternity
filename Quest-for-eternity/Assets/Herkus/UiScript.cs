using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

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


    private void Start()
    {
        uiTextArray = GetComponentsInChildren<TextMeshProUGUI>();
        UpdateTurnInfo(0);
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
