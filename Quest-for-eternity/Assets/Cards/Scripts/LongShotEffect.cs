using UnityEngine;

[CreateAssetMenu(fileName = "LongShotEffect", menuName = "Effect/LongShotEffect")]
[System.Serializable]
public class LongShotEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;
        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.isInLongShotMode = true;
            //inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.DisableAllCardsEvent();
            //inputGameobject.GetComponent<SceneObjectDatabase>().handScriptReference.SetCardActivityStatus(true, 0);
            //inputGameobject.GetComponent<SceneObjectDatabase>().playerStatReference.playerHealthOffset += (int)effectValue;
        }
    }
}