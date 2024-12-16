using UnityEngine;

public class OnHoverScript : MonoBehaviour
{
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Vector3 moveVector = new Vector3(0.0f, 2.0f, 1.0f);
    public GameObject description;
    MeshRenderer myMeshRenderer;
    private void Awake()
    {
        initialScale = transform.root.transform.localScale;
        initialPosition = transform.root.transform.localPosition;
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
        Vector3 finalPosition = initialPosition;
        if (status) 
        {
            finalScale = initialScale * 2f;
            finalPosition = initialPosition + moveVector;
        }
        transform.root.transform.localScale = finalScale;
        transform.root.transform.position = finalPosition;
        myMeshRenderer.enabled = status;
        //description.SetActive(status);
    }
}
