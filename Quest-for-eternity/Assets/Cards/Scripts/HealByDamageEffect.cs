using UnityEngine;


[CreateAssetMenu(fileName = "HealByDamageEffect", menuName = "Effect/HealByDamageEffect")]
[System.Serializable]
public class HealByDamageEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.shouldHealByDamageAmount = true;
            //inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.damageMultiplier = (int)effectValue;
        }
    }
}
