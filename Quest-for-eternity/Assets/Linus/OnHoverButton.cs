using UnityEngine;
using UnityEngine.EventSystems;

public class OnButtonHover: MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] AudioClip hoverClip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundFXManager.instance.PlaySoundClip(hoverClip, transform, 2f, false);
    }
}