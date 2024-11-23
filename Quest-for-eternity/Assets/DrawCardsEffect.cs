using UnityEngine;

[CreateAssetMenu(fileName = "DrawCardsEffect", menuName = "Effect/DrawCardsEffect")]
[System.Serializable]
public class DrawCardsEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, int effectValue, T value)
    {
        Debug.Log("damage boost activated");
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.AddCardsToHand(effectValue);
        }
    }
}