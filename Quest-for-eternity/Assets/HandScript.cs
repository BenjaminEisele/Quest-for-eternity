using UnityEngine;

public class HandScript : MonoBehaviour
{
    [SerializeField]
    GameObject baseCard;

    [SerializeField]
    Transform cardSpawnLocator;

    [SerializeField]
    int cardCount;
    void Start()
    {
        Debug.Log("1");
        Vector3 cardPlacementVector = new Vector3(1,0,0);
        for(int i = 0; i < cardCount; i++)
        {
            GameObject cardClone = Instantiate(baseCard, cardSpawnLocator.position + cardPlacementVector, Quaternion.identity);
            cardPlacementVector += new Vector3(2, 0, 0);
            cardClone.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //  Physics.Raycast(ray, out RaycastHit hitInfo)
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                
                //Debug.Log(hit.transform.name);
              //  Debug.Log("hit");
            }
        }
       
    }
}
