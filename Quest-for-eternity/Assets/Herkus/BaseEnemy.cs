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
    public int lootCardId;

    public virtual int GenerateAttack()
    {
        return enemyDamage;
    }
}
