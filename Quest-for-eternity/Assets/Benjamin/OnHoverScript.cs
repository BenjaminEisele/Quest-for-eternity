using UnityEngine;

public class OnHoverScript : MonoBehaviour
{
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Vector3 moveVector = new Vector3(0.0f, 2.0f, 1.0f);
    public float zLocator;
    public GameObject description;
    MeshRenderer myMeshRenderer;
    [SerializeField]
    DragDrop dragDropAccess;

    private void Awake()
    {
        //savedVector = transform.parent.position;
        //initialScale = transform.root.transform.localScale;
        //initialPosition = transform.root.transform.localPosition;
        initialScale = transform.localScale;
        //initialPosition = transform.localPosition;
        myMeshRenderer = description.GetComponent<MeshRenderer>();
        dragDropAccess = GetComponent<DragDrop>();
        myMeshRenderer.enabled = false;
    }

    private void OnMouseEnter()
    {
        if (!dragDropAccess.isDragging) IncreasScale(true);
    }

    private void OnMouseExit()
    {
        IncreasScale(false);
    }

    public void IncreasScale(bool status)
    {
        Vector3 finalScale = initialScale;
        Vector3 finalPosition = initialPosition;
        if (status) 
        {
            transform.parent.position += new Vector3(0,0,-2);
            finalScale = initialScale * 2f;
            finalPosition = initialPosition + moveVector;
        }
        else
        {
            transform.parent.position = new Vector3(transform.parent.position.x, transform.parent.position.y, zLocator);
        }
        //transform.root.transform.localScale = finalScale;
       // transform.root.transform.position = finalPosition;
        transform.localScale = finalScale;
        //transform.localPosition = finalPosition;
        myMeshRenderer.enabled = status;
        //description.SetActive(status);
    }
}
