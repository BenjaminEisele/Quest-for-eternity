using UnityEngine;

[CreateAssetMenu(fileName = "DamageBoostEffect", menuName = "Effect/DamageBoostEffect")]
[System.Serializable]
public class DamageBoostEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        FieldScript.damagePoints += (int)effectValue;
    }
}
