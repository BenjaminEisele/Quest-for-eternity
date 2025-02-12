using UnityEngine;

[CreateAssetMenu(fileName = "SupportEffect", menuName = "Effect/SupportEffect")]
[System.Serializable]
public class SupportEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            SceneObjectDatabase sceneObjectDatabaseAccess = inputGameobject.GetComponent<SceneObjectDatabase>();
            sceneObjectDatabaseAccess.playerScriptAccess.PlayCardForOtherPlayer(0);
        }
    }
}
