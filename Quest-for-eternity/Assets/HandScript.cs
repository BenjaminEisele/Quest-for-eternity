using UnityEngine;

public class HandScript : MonoBehaviour
{
    [SerializeField]
    GameObject baseCard;

    [SerializeField]
    int cardCount;
    void Start()
    {
        for(int i = 0; i < cardCount; i++)
        {
           // Instantiate(baseCard, new Vector3(0,0,0), );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
