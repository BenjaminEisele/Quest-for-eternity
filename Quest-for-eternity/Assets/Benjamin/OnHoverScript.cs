using UnityEngine;

public class OnHoverScript : MonoBehaviour
{
    private Vector3 initialScale;
    public GameObject description;
    MeshRenderer myMeshRenderer;
    private void Awake()
    {
        initialScale = transform.root.transform.localScale;
        myMeshRenderer = description.GetComponent<MeshRenderer>();
        myMeshRenderer.enabled = false;
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
        myMeshRenderer.enabled = status;
        //description.SetActive(status);
    }
}
