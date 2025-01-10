using UnityEngine;
using Mirror;

public class Test : NetworkBehaviour
{
    [SyncVar]
    public int Health = 10;

    public static Test instance;

    private void Awake()
    {
        if(instance == null) { instance = this;}
    }

    public void SubtractHealth()
    {
        Health = -1;
    }
}
