using UnityEngine;
using UnityEngine.EventSystems;

public class QualityDropdown : MonoBehaviour, IPointerClickHandler
{
    private bool gameOpening = true;
    public TMPro.TMP_Dropdown qualityDropdown;
    public int qualityIndexSave;

    void Start()
    {
        if (PlayerPrefs.GetInt("quality") != 0 && PlayerPrefs.GetInt("quality") != 1 && PlayerPrefs.GetInt("quality") != 2 && PlayerPrefs.GetInt("quality") != 3)
        {
            PlayerPrefs.SetInt("quality", 1);
        }
        qualityDropdown.value = PlayerPrefs.GetInt("quality");
        gameOpening = false;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        SoundFXManager.instance.DropdownSound();
    }

    public void SetQuality(int qualityIndex)
    {
        if (!gameOpening)
        {
            SoundFXManager.instance.DropdownSound();
        }
        QualitySettings.SetQualityLevel(qualityIndex);
        qualityIndexSave = qualityIndex;
        PlayerPrefs.SetInt("quality", qualityIndexSave);
    }
}
