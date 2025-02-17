using UnityEngine;


[CreateAssetMenu(fileName = "DamageSliderEffect", menuName = "Effect/DamageSliderEffect")]
[System.Serializable]
public class DamageSliderEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            SceneObjectDatabase sceneObjectDatabaseAccess = inputGameobject.GetComponent<SceneObjectDatabase>();
            sceneObjectDatabaseAccess.handScriptReference.isInDamageSliderMode = true;
        }

    }
}