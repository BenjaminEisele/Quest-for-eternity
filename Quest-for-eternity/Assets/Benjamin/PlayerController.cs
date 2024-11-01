using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    public GameObject PlayerModel;

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (!PlayerModel.activeSelf)
            {
                //For every Player & only once
                PlayerModel.SetActive(true);
            } 
            
            if (isOwned)
            {
                //Every Tick
                //Call actual Game functions
            }
        }
    }

}
