using UnityEngine;


[CreateAssetMenu(fileName = "HammerEffect", menuName = "Effect/HammerEffect")]
[System.Serializable]
public class HammerEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().playerScriptAccess.DealDamagePlayerScript(false, false, (int)effectValue, true, false);
        }
    }
}