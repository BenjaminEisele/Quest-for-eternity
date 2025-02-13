using UnityEngine;
using DG.Tweening;
using Mirror.Examples.AdditiveLevels;

public class DoorZoom : MonoBehaviour
{
    [SerializeField]
    SteamLobby steamLobbyAccess;
    public void DoDoorZoom()
    {
        Vector3 vec = new Vector3(2,2,2);
        transform.DOScale(vec, 1f).OnComplete(steamLobbyAccess.HostLobby);
    }
}
