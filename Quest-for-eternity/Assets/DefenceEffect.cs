using UnityEngine;


[CreateAssetMenu(fileName = "DefenceEffect", menuName = "Effect/DefenceEffect")]
[System.Serializable]
public class DefenceEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.playerHealthOffset += (int)effectValue;
           // inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.ChangeAllVisualHitrates(true, 0);
        }
    }
}