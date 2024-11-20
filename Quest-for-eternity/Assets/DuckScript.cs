using UnityEngine;

public class DuckScript : AnimalScript
{
    /* public override void SampleMethod()
     {

     }*/
    public override void Attack(int damage)
    {
        Debug.Log("i am a duck");
    }
    // Update is called once per frame
    void Start()
    {
        Attack(2);
    }
}
