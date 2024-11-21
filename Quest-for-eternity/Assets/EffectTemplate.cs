using UnityEngine;

public abstract class EffectTemplate : ScriptableObject
{
    public virtual void UseEffect<T>(int targetId, T value)
    {
        Debug.Log("I dun it!");
    }
}

[CreateAssetMenu(fileName = "HealingEffect", menuName = "Effect/HealingEffect")]
public class HealingEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, T value)
    {
        Debug.Log($"I healed {targetId}! The value was: {value}");
    }
}

[CreateAssetMenu(fileName = "DamageBoostEffect", menuName = "Effect/DamageBoostEffect")]
public class DamageBoostEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, T value)
    {
        Debug.Log($"I boosted the damage by {targetId}! The value was: {value}");
    }
}