using UnityEngine;
using DG.Tweening;
using Mirror.Examples.AdditiveLevels;

public class DoorZoom : MonoBehaviour
{
    [SerializeField]
    SteamLobby steamLobbyAccess;
    public void DoDoorZoom()
    {
        transform.DOScale(new Vector3(2, 2, 2), 1f);
        steamLobbyAccess.HostLobby();
    }
}
