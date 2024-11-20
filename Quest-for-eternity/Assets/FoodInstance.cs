using UnityEngine;

public class FoodInstance : MonoBehaviour
{
    public int id;

    void Start()
    {
        AssignBehaviorBasedOnID();
    }

    void AssignBehaviorBasedOnID()
    {
        switch (id)
        {
            case 1:
                //PoisonEffect myEffect = gameObject.AddComponent<PoisonEffect>();
                //myEffect.ExecuteEffect(0);


                break;
            case 2:
              //  gameObject.AddComponent<IceCardBehavior>();
                break;
            default:
                //gameObject.AddComponent<DefaultCardBehavior>();
                break;
        }
    }
}
