using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Tutorial : MonoBehaviour
{
    public Sprite[] slides;
    public UnityEngine.UI.Image image;
    private int currentSlide = 0;

    public void NextSlide()
    {
        if (currentSlide < slides.Length -1)
        {
            currentSlide++;
            image.sprite = slides[currentSlide]; 
            VoiceManager.instance.PlayTutorialLine(currentSlide);
        }
    }

    public void PreviousSlide()
    {
        if (currentSlide > 0)
        {
            currentSlide--;
            image.sprite = slides[currentSlide];  
            VoiceManager.instance.PlayTutorialLine(currentSlide);
        }
    }

    public void ResetTutorial()
    {
        currentSlide = 0;
        image.sprite = slides[currentSlide];
    }
}