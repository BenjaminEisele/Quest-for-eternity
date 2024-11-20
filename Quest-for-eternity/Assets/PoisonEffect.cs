using UnityEngine;

public class PoisonEffect : BaseEffect, IEffect
{

    //override public void  Effect2(int damage)
  //  {
  //      Debug.Log(damage);
  //  }

    private void Start()
    {
        //Effect2(3);
        //Effect2(100);
    }
      public void ExecuteEffect<T>(T ipnut)
      {
          Debug.Log($"I did {ipnut} poison damage");
      } 
}
