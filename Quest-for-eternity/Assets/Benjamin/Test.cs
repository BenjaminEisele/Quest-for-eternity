using UnityEngine;
using Mirror;

public class Test : NetworkBehaviour
{
    [SyncVar]
    public int Health;

    public static Test instance;

    private void Awake()
    {
        if(instance == null) { instance = this;}
        Health = 10;
    }

    public void SubtractHealth()
    {
        Health -= 1;
    }
}
