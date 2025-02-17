using UnityEngine;

[CreateAssetMenu(fileName = "ImmunityEffect", menuName = "Effect/ImmunityEffect")]
[System.Serializable]
public class ImmunityEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.immunityIdList.Add((int)effectValue);
            inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.immunityCount += 2;
            //inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.DealDamagePlayerScript(false, true, (int)effectValue, false, true);
        }
    }
}
