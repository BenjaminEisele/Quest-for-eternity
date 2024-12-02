using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Database : MonoBehaviour
{
    public List<DataUnit> cardDatabase;
    public List<BaseCard> cardList;
    public List<BaseEnemy> enemyList;

    /* private void Start()
     {
         Debug.Log("hello");

         Utility ut = cardList[0] as Utility;
         if (ut)
         {
             ut.effectList[0].UseEffect<string>(123, "asdf");
         } 
     } */
}
