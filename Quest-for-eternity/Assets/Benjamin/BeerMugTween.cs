using UnityEngine;
using Mirror;
using DG.Tweening;

public class BeerMugTween : NetworkBehaviour
{
    public void ReadyTween(bool server)
    {
        if (server)
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

    public void ResetTween(bool server)
    {
        if (server)
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
        transform.DOLocalMoveX(760, 1.5f);
    }

    private void ResetServerTween()
    {
        transform.DOLocalMoveX(0, 0.75f);
    }

    private void ClientTween()
    {
        transform.DOLocalRotate(new Vector3(0, 180, 0), 0f);
        transform.DOLocalMoveX(-810, 1.5f);
    }

    private void ResetClientTween()
    {
        transform.DOLocalMoveX(0, 0.75f);
    }
}
