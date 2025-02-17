using UnityEngine;

[CreateAssetMenu(fileName = "HealingMultiplierEffect", menuName = "Effect/HealingMultiplierEffect")]
[System.Serializable]
public class HealingMultiplierEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            //inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.multiplier = (int)effectValue;
            inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.healingMultiplier = (int)effectValue;
        }
    }
}
