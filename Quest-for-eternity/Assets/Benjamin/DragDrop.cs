using UnityEngine;

public class DragDrop : MonoBehaviour
{
    Vector3 mousePositionOffset;
    Vector3 cardPosition;
    bool isInPlayingField;
    [SerializeField]
    HandScript handScriptAccess;

    private void Start()
    {
        cardPosition = transform.position;
    }

    private Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
    }

    private void OnMouseUp()
    {
        if (isInPlayingField)
        {
            handScriptAccess.PlayCard();
        }
        else
        {
            transform.position = cardPosition;
        }
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + mousePositionOffset;
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

