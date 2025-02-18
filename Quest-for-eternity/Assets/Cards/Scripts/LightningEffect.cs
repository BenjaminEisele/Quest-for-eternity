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
            FieldScript.damagePoints += RefereeScript.instance.enemyList.Count;
            inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.DealDamagePlayerScript(false, true, 0, false, true);
        }
    }
}
