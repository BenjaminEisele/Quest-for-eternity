using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DragDrop : MonoBehaviour
{
    Vector3 mousePositionOffset;
    [HideInInspector]
    public Vector3 cardPosition;
    bool isInPlayingField;
    bool isInSendCardsOverField;
    bool isInDiscardField;
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
            if (!transform.GetComponentInParent<CardScript>().isActionCard)
            {
                if (handScriptAccess.utilityCount < handScriptAccess.utilityLimit)
                {
                    handScriptAccess.PlayCard(transform);
                }
                else
                {
                    transform.localPosition = cardPosition;
                }
            }
            else
            {
                handScriptAccess.PlayCard(transform);
            }
        }
        else if (isInSendCardsOverField)
        {
            if (!transform.GetComponentInParent<CardScript>().isActionCard)
            {
                if (handScriptAccess.utlCardsPlayedForOtherPlayer < 3)
                {
                    handScriptAccess.SendCardsOver(transform);
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
        else if (isInDiscardField)
        {
            handScriptAccess.DiscardCard(transform);
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
        else if (col.gameObject.name == "SendCardsOverField")
        {
            isInSendCardsOverField = true;
        }
        else if (col.gameObject.name == "DiscardCardField")
        {
            isInDiscardField = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.name == "PlayingField")
        {
            isInPlayingField = false;
        }

        else if (col.gameObject.name == "SendCardsOverField")
        {
            isInSendCardsOverField = false;
        }

        else if (col.gameObject.name == "DiscardCardField")
        {
            isInDiscardField = false;
        }
    }
}

