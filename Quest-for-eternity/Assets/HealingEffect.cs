using UnityEngine;

[CreateAssetMenu(fileName = "HealingEffect", menuName = "Effect/HealingEffect")]
[System.Serializable]
public class HealingEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, T value)
    {
        Debug.Log($"I healed {targetId}! The value was: {value}");
    }
}
