using UnityEngine;

public class SoundFXScript : MonoBehaviour
{
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip sliderSound;

    public void ClickSound()
    {
        SoundFXManager.instance.PlaySoundClip(buttonSound, transform, 1f, false);
    }

        public void BubbleSound()
    {

        SoundFXManager.instance.PlaySoundClip(sliderSound, transform, 1f, true);
    }

    

}
