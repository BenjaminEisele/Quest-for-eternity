using UnityEngine;


[CreateAssetMenu(fileName = "HitRateSetEffect", menuName = "Effect/HitRateSetEffect")]
[System.Serializable]
public class HitRateSetEffect : EffectTemplate
{
    HandScript handScriptAccess;
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().fieldScriptReference.hitRateModifier = effectValue;
            handScriptAccess = inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference;
            handScriptAccess.ChangeAllVisualHitrates(false, effectValue, true);
        }
    }

    private void OnDestroy()
    {
        handScriptAccess.HitRateRestoriationMethod();
    }
}
