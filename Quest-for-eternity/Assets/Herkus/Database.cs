using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Database : MonoBehaviour
{
    public List<DataUnit> cardDatabase;
    public List<BaseCard> cardList;
    public List<BaseEnemy> enemyList;

    public static Database instance;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

}
