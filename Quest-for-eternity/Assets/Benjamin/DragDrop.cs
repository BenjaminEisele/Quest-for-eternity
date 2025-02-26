using UnityEngine;
using DG.Tweening;


public class DragDrop : MonoBehaviour
{
    Vector3 mousePositionOffset;
    [HideInInspector]
    public Vector3 cardPosition;
    public bool isInPlayingField;
    bool isInSendCardsOverField;
    bool isInDiscardField;
    [SerializeField]
    HandScript handScriptAccess;
    [SerializeField]
    CardScript cardScriptAccess;
    [SerializeField]
    OnHoverScript onHoverScriptAccess;
    [SerializeField]
    PauseMenuCheck pauseMenuCheckAccess;
    [HideInInspector]
    public bool isDragging = false;
    public Transform rootParent;
    [SerializeField]
    Transform scaleParent;
    float animationSpeed = 0.25f;
    private void Start()
    {
        cardPosition = transform.localPosition;
        onHoverScriptAccess = GetComponent<OnHoverScript>();
        cardScriptAccess = GetComponent<CardScript>();
    }
    private void Awake()
    {
        rootParent = transform.root; 
    }
    private Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDown()
    {
        if (!pauseMenuCheckAccess.pauseMenuOpen && handScriptAccess.canInteract)
        {
            //scaleParent.DOLocalRotate(new Vector3(0, 0, -rootParent.eulerAngles.z), animationSpeed);
            scaleParent.localEulerAngles = new Vector3(0, 0, -rootParent.eulerAngles.z);
            //DOTween.KillAll();
            DOTween.Kill(8);
            mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
            isDragging = true;
            onHoverScriptAccess.IncreasScale(false);
        }
    }

    private void OnMouseUp()
    {
        scaleParent.localEulerAngles = new Vector3(0, 0, 0);
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
                    handScriptAccess.SendCardsOver(transform, -1);
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
        if(cardScriptAccess.isClickable && handScriptAccess.canInteract)
        {
            transform.position = GetMouseWorldPosition() + mousePositionOffset;
            //transform.position = new Vector3(0, 0, -rootParent.eulerAngles.z);
            scaleParent.localEulerAngles = new Vector3(0, 0, -rootParent.eulerAngles.z);
            // DOTween.KillAll();
            DOTween.Kill(8);

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

