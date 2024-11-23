using UnityEngine;

[CreateAssetMenu(fileName = "DrawCardsEffect", menuName = "Effect/DrawCardsEffect")]
[System.Serializable]
public class DrawCardsEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        Debug.Log("damage boost activated");
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.AddCardsToHand((int)effectValue);
        }
    }
}