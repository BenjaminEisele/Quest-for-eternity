using UnityEngine;
using Mirror;
using DG.Tweening;

public class BeerMugTween : NetworkBehaviour
{
    public void ReadyTween()
    {
        if (!isClientOnly)
        {
            ServerTween();
            Debug.Log("ServerTween");
        }
        else
        {
            ClientTween();
            Debug.Log("ClientTween");
        }
    }

    public void ResetTween()
    {
        if (!isClientOnly)
        {
            ResetServerTween();
        }
        else
        {
            ResetClientTween();
        }
    }

    private void ServerTween()
    {
        transform.DOLocalMoveX(755, 1.5f);
    }

    private void ResetServerTween()
    {
        transform.DOLocalMoveX(0, 0.75f);
    }

    private void ClientTween()
    {
        transform.DOLocalMoveX(-800, 1.5f);
    }

    private void ResetClientTween()
    {
        transform.DOLocalMoveX(0, 0.75f);
    }
}
