using UnityEngine;


[CreateAssetMenu(fileName = "DoubleDamageEffect", menuName = "Effect/DoubleDamageEffect")]
[System.Serializable]
public class DoubleDamageEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.multiplier = (int)effectValue;
            inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.damageMultiplier = (int)effectValue;
        }
    }
}
