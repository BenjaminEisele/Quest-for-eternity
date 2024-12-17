using UnityEngine;

public class DragDrop : MonoBehaviour
{
    Vector3 mousePositionOffset;
   // [HideInInspector]
    public Vector3 cardPosition;
    public bool isInPlayingField;
    [SerializeField]
    HandScript handScriptAccess;
    [SerializeField]
    CardScript cardScriptAccess;
    [SerializeField]
    OnHoverScript onHoverScriptAccess;
    public bool isDragging = false;

    private void Start()
    {
        cardPosition = transform.localPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
        isDragging = true;
        onHoverScriptAccess.IncreasScale(false);
    }

    private void OnMouseUp()
    {
        isDragging = false;
        if (isInPlayingField)
        {
            handScriptAccess.PlayCard();
        }
        else
        {
            transform.localPosition = cardPosition;
        }
    }

    private void OnMouseDrag()
    {
        if (!handScriptAccess.isInQuickAttackMode)
        {
            transform.position = GetMouseWorldPosition() + mousePositionOffset;
        }
        else if (handScriptAccess.isInQuickAttackMode && cardScriptAccess.isActionCard)
        {
            transform.position = GetMouseWorldPosition() + mousePositionOffset;
        }       
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "PlayingField")
        {
            isInPlayingField = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.name == "PlayingField")
        {
            isInPlayingField = false;
        }
    }
}

