using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageSliderScript : MonoBehaviour
{
    public int sliderDamage;
    [SerializeField]
    FieldScript fieldScriptAccess;
    [SerializeField]
    TurnScript turnScriptAccess;
    [SerializeField]
    PlayerStatScript playerStatAccess;
    public Slider mySlider;
    public TextMeshProUGUI myTextMesh;
    public Button myButton;
    public int fakeSliderValue;

    private void Start()
    {
        //mySlider = GetComponentInChildren<Slider>();
    }
    private void OnEnable()
    {
        //Debug.Log("enabled");
        myButton.enabled = true;
        mySlider.minValue = 1 + FieldScript.boostPoints;
        mySlider.maxValue = 5 + FieldScript.boostPoints;
    }
    public void SetSliderDamage()
    {
        sliderDamage = (int)mySlider.value;
        fakeSliderValue = sliderDamage - FieldScript.boostPoints;
        
        myTextMesh.text = sliderDamage.ToString();
    }
    
    public void ConfirmDamage()
    {
        FieldScript.damagePoints = fakeSliderValue;
        playerStatAccess.ChangeHealthNest(-fakeSliderValue, 0, true);
        myButton.enabled = false;
        turnScriptAccess.CallEndTurnEvent();
        gameObject.SetActive(false);
    }
}
