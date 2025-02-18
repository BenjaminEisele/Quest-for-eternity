using UnityEngine;
using UnityEngine.EventSystems;

public class OnButtonHover: MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] SoundFXManager soundFXManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        soundFXManager.HoverSound();
    }
}