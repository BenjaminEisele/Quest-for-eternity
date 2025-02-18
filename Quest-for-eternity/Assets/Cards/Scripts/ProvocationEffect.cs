using UnityEngine;

[CreateAssetMenu(fileName = "ProvocationEffect", menuName = "Effect/ProvocationEffect")]
[System.Serializable]

public class ProvocationEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        RefereeScript.instance.SwitchPlayerAttackIdNest();
        RefereeScript.instance.shouldSwitchTargetPlayer = false;
    }
}