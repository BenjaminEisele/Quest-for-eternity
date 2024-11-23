using UnityEngine;

[CreateAssetMenu(fileName = "HitRateEffect", menuName = "Effect/HitRateEffect")]
[System.Serializable]
public class HitRateEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            //inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.SetUtilityCardStatus(false);
            inputGameobject.GetComponent<SceneObjectDatabase>().fieldScriptReference.hitRateModifier += effectValue;
        }
    }
}
