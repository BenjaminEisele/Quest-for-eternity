using UnityEngine;

public class OnHoverScript : MonoBehaviour
{
    private Vector3 initialScale;
    public GameObject description;

    private void Awake()
    {
        initialScale = transform.root.transform.localScale;
    }

    private void OnMouseEnter()
    {
        IncreasScale(true);
    }

    private void OnMouseExit()
    {
        IncreasScale(false);
    }

    private void IncreasScale(bool status)
    {
        Vector3 finalScale = initialScale;
        if (status) {finalScale = initialScale * 2f;}
        transform.root.transform.localScale = finalScale;
        description.SetActive(status);
    }
}
