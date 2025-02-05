using UnityEngine;

[CreateAssetMenu(fileName = "UtilityLimitIncreaseEffect", menuName = "Effect/UtilityLimitIncreaseEffect")]
[System.Serializable]
public class UtilityLimitIncreaseEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        //Debug.Log("damage boost activated");
        GameObject inputGameobject = value as GameObject;
        inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.utilityLimit = (int)effectValue;

    }
}
