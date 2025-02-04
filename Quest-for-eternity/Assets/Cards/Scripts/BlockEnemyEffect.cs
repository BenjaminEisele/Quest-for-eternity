using UnityEngine;

[CreateAssetMenu(fileName = "BlockEnemyEffect", menuName = "Effect/BlockEnemyEffect")]
[System.Serializable]
public class BlockEnemyEffect : EffectTemplate
{
    public override void UseEffect<T>(int targetId, float effectValue, T value)
    {
        GameObject inputGameobject = value as GameObject;

        if (inputGameobject.GetComponent<SceneObjectDatabase>() != null)
        {
            Debug.Log($"{RefereeScript.instance.enemyList[targetId].enemyNameText.text} is blocked");
            RefereeScript.instance.enemyList[targetId].canAttack = false;
            /*SceneObjectDatabase sceneObjectDatabaseAccess = inputGameobject.GetComponent<SceneObjectDatabase>();
            // sceneObjectDatabaseAccess.playerStatReference.playerHealth += (int)effectValue;
            sceneObjectDatabaseAccess.playerStatReference.ChangePlayerHealth((int)effectValue);
            //Debug.Log(sceneObjectDatabaseAccess.playerStatReference.playerHealth); */
        }
        else
        {
            Debug.Log("nepavyko");
        }
        //Debug.Log($"I healed {targetId}! The value was: {value}");
    }
}
