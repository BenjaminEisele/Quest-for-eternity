using UnityEngine;

[CreateAssetMenu(fileName = "LightningEffect", menuName = "Effect/LightningEffect")]
[System.Serializable]
public class LightningEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.DealDamagePlayerScript(false, true, RefereeScript.instance.enemyList.Count, false, true);
        }
    }
}
