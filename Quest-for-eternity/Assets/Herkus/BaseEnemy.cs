using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "BaseEnemy", menuName = "Enemy/BaseEnemy")]
[System.Serializable]
public class BaseEnemy : ScriptableObject
{
    public string enemyName;
    public int enemyHealth;
    public int enemyDamage;
    public Sprite enemySprite;
    public bool isBoss;

    public virtual int GenerateAttack()
    {
        return enemyDamage;
    }
    /*public virtual void CallAttack<T>(int targetId, float effectValue, T value)
    {
        Debug.Log("I dun it!");
    } */
}
