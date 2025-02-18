using UnityEngine;

[CreateAssetMenu(fileName = "AoeEffect", menuName = "Effect/AoeEffect")]
[System.Serializable]
public class AoeEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        //var myVar = value;
        GameObject inputGameobject = value as GameObject;
        
        if(inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            SceneObjectDatabase sceneObjectDatabaseAccess = inputGameobject.GetComponent<SceneObjectDatabase>();
           // sceneObjectDatabaseAccess.playerStatReference.playerHealth += (int)effectValue;
            sceneObjectDatabaseAccess.playerStatReference.ChangePlayerHealth((int)effectValue);
            //Debug.Log(sceneObjectDatabaseAccess.playerStatReference.playerHealth);
        }
        else
        {
            Debug.Log("nepavyko");
        }
        //Debug.Log($"I healed {targetId}! The value was: {value}");
    }
}
