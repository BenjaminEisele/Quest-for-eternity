using UnityEngine;

[CreateAssetMenu(fileName = "MutationEffect", menuName = "Effect/MutationEffect")]
[System.Serializable]
public class MutationEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            SceneObjectDatabase sceneObjectDatabaseAccess = inputGameobject.GetComponent<SceneObjectDatabase>();
            sceneObjectDatabaseAccess.playerStatReference.CallMutation();
        }

    }
}