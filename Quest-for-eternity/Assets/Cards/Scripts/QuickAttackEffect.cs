using UnityEngine;

[CreateAssetMenu(fileName = "QuickAttackEffect", menuName = "Effect/QuickAttackEffect")]
[System.Serializable]
public class QuickAttackEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.SetCardActivityStatus(false, 0);
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.isInQuickAttackMode = true;
        }
    }
}