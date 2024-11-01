using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiScript : MonoBehaviour
{
    [SerializeField]
    static TextMeshPro fieldDamageText;

    [SerializeField]
    static TextMeshPro actionWindowText;

    [SerializeField]
    static TextMeshPro turnInfoText;

    [SerializeField]
    static TextMeshPro[] uiTextArray;

    private void Start()
    {
        //fieldDamageText = GetComponentInChildren<TextMeshPro>();
        //turnInfoText = GetComponentInChildren<TextMeshPro>();
        uiTextArray = GetComponentsInChildren<TextMeshPro>();
      

    }
    public static void UpdateFighterText(TextMeshPro changedText, int value)
    {
        changedText.text = value.ToString();
    }

    public static void UpdateTurnInfo(int inputInfo)
    {
        string turnInfoString;
        if(inputInfo == 0)
        {
            turnInfoString = "your";
        }
        {
            turnInfoString = "the enemy's";
        }
        uiTextArray[1].text = $"It is {turnInfoString} turn!";
    }

    public static void UpdateFieldDamageText(int value)
    {
        uiTextArray[0].text = "Total damage: " + value.ToString();
    }

    public static void UpdateActionWindow(string inputString)
    {
        uiTextArray[1].text = inputString;
    }
}
