using UnityEngine;

[CreateAssetMenu(fileName = "DrawCardsEffect", menuName = "Effect/DrawCardsEffect")]
[System.Serializable]
public class DrawCardsEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {       
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.AddCardsToHand((int)effectValue);
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.RebuildCardListLite();
            //inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.transform.root.GetComponentInChildren<PlayerScript>()[]
            if(inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.isThisPlayersTurn)
            {
                inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.ActivateAllCardsEvent();
            }
            else
            {
                inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.DisableAllCardsEvent();
            }
            
        }
    }
}