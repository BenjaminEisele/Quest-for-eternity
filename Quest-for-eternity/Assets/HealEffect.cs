using UnityEngine;


[CreateAssetMenu(fileName = "HealEffect", menuName = "HealEffectScriptable")]

public class HealEffect : BaseEffect, IEffect
{

    override public void ExecuteEffect<T>(T ipnut)
    {
        Debug.Log("A0");
        Debug.Log($"I healed {ipnut} points");
    }
}
