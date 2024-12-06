using UnityEngine;

public class RestartButtonScript : MonoBehaviour
{
    public GameObject restartButton;
    public GameObject winImage;
    public GameObject lostImage;

    public void RestartGame()
    {
        restartButton.SetActive(false);
        winImage.SetActive(false);
        lostImage.SetActive(false);
    }
}
