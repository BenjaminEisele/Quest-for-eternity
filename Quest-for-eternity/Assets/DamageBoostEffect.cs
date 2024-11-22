using UnityEngine;


[CreateAssetMenu(fileName = "DamageBoostEffect", menuName = "Effect/DamageBoostEffect")]
[System.Serializable]
public class DamageBoostEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, T value)
    {
        Debug.Log($"I boosted the damage by {targetId}! The value was: {value}");
    }
}
