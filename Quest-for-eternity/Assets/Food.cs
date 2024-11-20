using UnityEngine;


public interface IFood
{
    void ExecuteEffect<T>(T input);
}
public class Food : MonoBehaviour//, FoodInterface
{
    public IFood myFoodInterface;
    int damage;

    public virtual void Effect2(int damage)
    {
        Debug.Log("hi");
    }
    
    public void TryAddEffect()
    {
        myFoodInterface?.ExecuteEffect(damage);
    }
}
