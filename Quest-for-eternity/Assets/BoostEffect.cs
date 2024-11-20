using UnityEngine;


[CreateAssetMenu(fileName = "BoostEffect", menuName = "BoostEffectScriptable")]

public class BoostEffect : BaseEffect, IEffect
{
    override public void ExecuteEffect<T>(T ipnut)
    {
        Debug.Log("A0");
        Debug.Log($"I boosted my damage by {ipnut} points");
    }
}