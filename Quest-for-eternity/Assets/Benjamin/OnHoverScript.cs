using UnityEngine;
using DG.Tweening;

public class OnHoverScript : MonoBehaviour
{
    private Vector3 initialScale;
    public float zLocator;
    public GameObject description;
    MeshRenderer myMeshRenderer;
    [SerializeField]
    DragDrop dragDropAccess;
    [SerializeField]
    Transform scaleParent;
    Transform rootParent;
    [SerializeField]
    float animationSpeed;
    [SerializeField]
    HandScript handScriptAccess;

    [SerializeField] SoundFXManager soundFXManager;

    private void Awake()
    {
        rootParent = transform.root;
        initialScale = scaleParent.localScale;
        myMeshRenderer = description.GetComponent<MeshRenderer>();
        dragDropAccess = GetComponent<DragDrop>();
        myMeshRenderer.enabled = false;
    }

    private void OnMouseEnter()
    {
        if (!dragDropAccess.isDragging && handScriptAccess.canInteract) IncreasScale(true);
    }

    private void OnMouseExit()
    {
        IncreasScale(false);
    }

    public void IncreasScale(bool status)
    {
        Vector3 finalScale = initialScale;
        if (status) 
        {
            soundFXManager.DrawSound();
            scaleParent.DOLocalRotate(new Vector3(0, 0, -rootParent.eulerAngles.z), animationSpeed);
            scaleParent.DOLocalMoveY(3, animationSpeed);
            transform.parent.position += new Vector3(0,0,-2);
            finalScale = initialScale * 2f;
            scaleParent.DOScale(finalScale, animationSpeed);
        }
        else
        {
            scaleParent.DOLocalRotate(new Vector3(0, 0, 0), animationSpeed);
            scaleParent.DOLocalMoveY(0, animationSpeed);
            scaleParent.DOScale(initialScale, animationSpeed);
            transform.parent.position = new Vector3(transform.parent.position.x, transform.parent.position.y, zLocator);
        }
        myMeshRenderer.enabled = status;
    }
}
