using UnityEngine;

[CreateAssetMenu(fileName = "AoeEffect", menuName = "Effect/AoeEffect")]
[System.Serializable]
public class AoeEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.shouldHealByDamageAmount = true;
        }

    }
}
