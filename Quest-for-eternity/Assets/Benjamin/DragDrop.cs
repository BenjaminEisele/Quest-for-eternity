using UnityEngine;

public class DragDrop : MonoBehaviour
{
    Vector3 mousePositionOffset;
    [HideInInspector]
    public Vector3 cardPosition;
    private bool isInPlayingField;
    private bool isInSendCardsOverField;
    [SerializeField]
    HandScript handScriptAccess;
    [SerializeField]
    CardScript cardScriptAccess;
    [SerializeField]
    OnHoverScript onHoverScriptAccess;
    [HideInInspector]
    public bool isDragging = false;

    private void Start()
    {
        cardPosition = transform.localPosition;
        onHoverScriptAccess = GetComponent<OnHoverScript>();
        cardScriptAccess = GetComponent<CardScript>();
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
        if (isInSendCardsOverField)
        {
            if (!transform.GetComponentInParent<CardScript>().isActionCard)
            {
                if (handScriptAccess.UtlCardsPlayedForOtherPlayer < 3)
                {
                    handScriptAccess.SendCardsOver();
                }
                else
                {
                    transform.localPosition = cardPosition;
                }                    
            }
            else
            {
                transform.localPosition = cardPosition;
            }
        }
        else
        {
            transform.localPosition = cardPosition;
        }
    }

    private void OnMouseDrag()
    {
        if(cardScriptAccess.isClickable)
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
        if (col.gameObject.name == "SendCardsOverField")
        {
            isInSendCardsOverField = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.name == "PlayingField")
        {
            isInPlayingField = false;
        }

        if (col.gameObject.name == "SendCardsOverField")
        {
            isInSendCardsOverField = false;
        }
    }
}

