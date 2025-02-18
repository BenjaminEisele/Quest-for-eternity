using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Tutorial : MonoBehaviour
{
    public Sprite[] slides;
    public UnityEngine.UI.Image image;
    [SerializeField] VoiceManager voiceManager;
    private int currentSlide = 0;

    void Start()
    {
        
    }

    public void NextSlide()
    {
        if (currentSlide < slides.Length -1)
        {
            currentSlide++;
            image.sprite = slides[currentSlide]; 
            voiceManager.PlayTutorialLine(currentSlide);
        }
    }

        public void PreviousSlide()
    {
        if (currentSlide > 0)
        {
            currentSlide--;
            image.sprite = slides[currentSlide];  
            voiceManager.PlayTutorialLine(currentSlide);
        }
    }
}