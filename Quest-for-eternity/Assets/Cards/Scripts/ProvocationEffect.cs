using UnityEngine;

[CreateAssetMenu(fileName = "ProvocationEffect", menuName = "Effect/ProvocationEffect")]
[System.Serializable]

public class ProvocationEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        RefereeScript.instance.SwitchPlayerAttackIdNest(true);
        RefereeScript.instance.shouldSwitchTargetPlayer = false;
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.GenerateProvocationIcon();
        }
    }
}