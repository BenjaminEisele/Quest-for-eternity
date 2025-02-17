using UnityEngine;

[CreateAssetMenu(fileName = "AreaAttackEffect", menuName = "Effect/AreaAttackEffect")]
[System.Serializable]
public class AreaAttackEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.DealDamagePlayerScript(false, true, (int)effectValue, false, true);
        }
    }
}
