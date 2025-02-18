using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverText: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject descText;

    void Start()
    {
        descText.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descText.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        descText.SetActive(false);
    }

    public void DeactivateText()
    {
        descText.SetActive(false);
    }
}