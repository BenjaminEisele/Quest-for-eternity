using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class DisplayCardHoverScript : MonoBehaviour
{
    [SerializeField]
    GameObject text;

    private void OnMouseEnter()
    {
        ShowDescription(true);
    }

    private void OnMouseExit()
    {
        ShowDescription(false);
    }

    private void ShowDescription(bool status)
    {
        text.SetActive(status);
    }
}
