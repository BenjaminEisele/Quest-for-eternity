using UnityEngine;


interface ISampleInterface
{
    void Attack(int damage);
    void SaySomething(string inputString);
}
public class AnimalScript : MonoBehaviour, ISampleInterface
{

    public virtual void Attack(int damage)
    {
        Debug.Log(damage);
    }
    public virtual void SaySomething(string inputString)
    {
        Debug.Log(inputString);
    }

}
