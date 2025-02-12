using UnityEngine;

[CreateAssetMenu(fileName = "HealingEffect", menuName = "Effect/HealingEffect")]
[System.Serializable]
public class HealingEffect : EffectTemplate
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
            if(sceneObjectDatabaseAccess.playerStatReference.playerHealth > 0)
            {
                sceneObjectDatabaseAccess.playerScriptAccess.isPlayerAlive = true;
            }
            //Debug.Log(sceneObjectDatabaseAccess.playerStatReference.playerHealth);
        }
        else
        {
            Debug.Log("nepavyko");
        }
        //Debug.Log($"I healed {targetId}! The value was: {value}");
    }
}
