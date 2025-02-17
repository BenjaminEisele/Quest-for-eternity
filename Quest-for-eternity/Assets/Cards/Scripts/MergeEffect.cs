using UnityEngine;


[CreateAssetMenu(fileName = "MergeEffect", menuName = "Effect/MergeEffect")]
[System.Serializable]
public class MergeEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        //var myVar = value;
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            SceneObjectDatabase sceneObjectDatabaseAccess = inputGameobject.GetComponent<SceneObjectDatabase>();
            sceneObjectDatabaseAccess.handScriptReference.isInMergeMode = true;
            sceneObjectDatabaseAccess.handScriptReference.SetCardActivityStatus(false, 0);
        }
    }
}