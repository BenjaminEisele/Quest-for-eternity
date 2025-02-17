using UnityEngine;

[CreateAssetMenu(fileName = "ArmorEffect", menuName = "Effect/ArmorEffect")]
[System.Serializable]
public class ArmorEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            SceneObjectDatabase sceneObjectDatabaseAccess = inputGameobject.GetComponent<SceneObjectDatabase>();
            sceneObjectDatabaseAccess.playerStatReference.ChangeHealthNest(0, (int)effectValue, true);
        }

    }
}