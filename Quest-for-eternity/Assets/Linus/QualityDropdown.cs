using UnityEngine;

public class QualityDropdown : MonoBehaviour
{
    public TMPro.TMP_Dropdown qualityDropdown;
    public int qualityIndexSave;

    void Start()
    {
        if (PlayerPrefs.GetInt("quality") != 0 && PlayerPrefs.GetInt("quality") != 1 && PlayerPrefs.GetInt("quality") != 2 && PlayerPrefs.GetInt("quality") != 3)
        {
            PlayerPrefs.SetInt("quality", 1);
        }
        qualityDropdown.value = PlayerPrefs.GetInt("quality");
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        qualityIndexSave = qualityIndex;
        PlayerPrefs.SetInt("quality", qualityIndexSave);
    }
}
