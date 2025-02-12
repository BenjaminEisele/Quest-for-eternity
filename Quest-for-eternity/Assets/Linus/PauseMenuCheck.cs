using UnityEngine;

public class PauseMenuCheck : MonoBehaviour
{
    public bool pauseMenuOpen = false;

    public void OpenPauseMenu ()
    {
        pauseMenuOpen = true;
    }

    public void ClosePauseMenu()
    {
        pauseMenuOpen = false;
    }
}

