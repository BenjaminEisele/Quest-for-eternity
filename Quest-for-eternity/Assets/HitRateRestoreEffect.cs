using UnityEngine;

[CreateAssetMenu(fileName = "HitRateRestoreEffect", menuName = "Effect/HitRateRestoreEffect")]
[System.Serializable]
public class HitRateRestoreEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            //inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.SetUtilityCardStatus(false);
            //inputGameobject.GetComponent<SceneObjectDatabase>().fieldScriptReference.hitRateModifier += effectValue;
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.ChangeAllVisualHitrates(true, 0);
        }
    }
}
