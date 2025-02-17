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
            foreach(EnemyScript enemy in RefereeScript.instance.enemyList)
            {
                enemy.canAttack = false;
            }
        }
        else
        {
            Debug.Log("nepavyko");
        }
    }
}
