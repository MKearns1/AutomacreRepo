using UnityEngine;

public class SoundInstance : MonoBehaviour
{
    AudioClip clip;
    public AudioSource source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(AudioClip clip = null)
    {
        this.clip = clip;
        source.clip = clip;
        source.Play();
        Destroy(gameObject, clip.length);
    }
}
