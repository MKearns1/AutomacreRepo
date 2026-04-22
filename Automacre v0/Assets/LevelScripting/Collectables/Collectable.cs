using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int Value = 1;
    public AudioClip sound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.GetMask("Bot"))
        {
            Destroy(Instantiate(VFXManager.instance.CollectableParticleVFX, transform.position, Quaternion.identity), 2);
            LevelEventsManager.instance.CollectedCollectable(Value);
            SoundManager.instance.PlaySound(sound);
            Destroy(gameObject);
        }
    }
}
