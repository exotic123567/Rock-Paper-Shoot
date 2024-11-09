using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip textwritersound;
    [FormerlySerializedAs("audioSource")] [SerializeField] private AudioSource BGMusicaudioSource;
    [SerializeField] private AudioSource textwriteraudiosource;
    [SerializeField] private AudioSource ammopickupaudiosrc;
    [SerializeField] private AudioSource bulletshooteraudiosrc;
    [SerializeField] private AudioSource ammotypeseleectionuisrc;
    public AudioClip backgroundMusicClip;

    private void Awake()
    {
        // Ensure there's only one AudioManager instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Make this persist between scene loads
        }
        else
        {
            Destroy(gameObject);
        }

        BGMusicaudioSource = GetComponent<AudioSource>();
    }

    public void PlayBGMusic()
    {
        if (!BGMusicaudioSource.isPlaying)
        {
            BGMusicaudioSource.Play();
            Debug.Log("Background music started playing.");
        }
    }

    public void StopBGMusic()
    {
        BGMusicaudioSource.Stop();
    }

    // Function to play hit sound effect
    public void PlayHitSound()
    {
        BGMusicaudioSource.PlayOneShot(hitSound);  // Plays the hit sound effect
    }

    public void PlayTextwriterSound()
    {
        textwriteraudiosource.Play();
    }

    public void StopTextwriterSound()
    {
        textwriteraudiosource.Stop();
    }

    public void PlayAmmoPickupSound()
    {
        ammopickupaudiosrc.Play();
    }

    public void StopAmmoPickupSound()
    {
        ammopickupaudiosrc.Stop();
    }

    public void PlayBulletshooterSound()
    {
        bulletshooteraudiosrc.Play();
    }

    public void StopBulletshooterSound()
    {
        bulletshooteraudiosrc.Stop();
    }

    public void PlayAmmoTypeseleectionSound()
    {
        ammotypeseleectionuisrc.Play();
    }

    public void StopAmmoTypeseleectionSound()
    {
        ammotypeseleectionuisrc.Stop();
    }
    
}