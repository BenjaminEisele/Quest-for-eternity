using Mirror;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    private GameObject Player1;
    private GameObject Player2;
    private GameObject Enemie;
    private int TurnOrder = 0;

    private void UpdateTurnOrder()
        {
            switch (TurnOrder) //later bools and then chekc in class
            {
                case 0:
                    Player1.SetActive(false);
                    Player2.SetActive(true);
                    break;

                case 1:
                    Player2.SetActive(false);
                    Enemie.SetActive(true);
                    break;

                case 2:
                    Enemie.SetActive(false);
                    Player1.SetActive(true);
                    break;
            }
        }

    private void Player1EndTurn()
    {
        TurnOrder = 1;
        UpdateTurnOrder();
    }

    private void Player2EndTurn()
    {
        TurnOrder = 2; 
        UpdateTurnOrder();
    }

    private void EnemieEndTurn()
    {
        TurnOrder= 0;
        UpdateTurnOrder();
    }

}
