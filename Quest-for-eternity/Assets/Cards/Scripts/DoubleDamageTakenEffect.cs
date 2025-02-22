using UnityEngine;


[CreateAssetMenu(fileName = "DoubleDamageTakenEffect", menuName = "Effect/DoubleDamageTakenEffect")]
[System.Serializable]
public class DoubleDamageTakenEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.damageMultiplier = (int)effectValue;
        }
    }
}
