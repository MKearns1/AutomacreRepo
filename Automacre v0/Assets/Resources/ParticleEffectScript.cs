using UnityEngine;

[ExecuteInEditMode]
public class ParticleEffectScript : MonoBehaviour
{
    ParticleSystem particle;
    public Color color;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particle = GetComponent<ParticleSystem>();

        var main = particle.main;
        main.startColor = color;

        var emis = particle.emission;
        emis.enabled = true;
        particle.Play();

    }

    // Update is called once per frame
    void Update()
    {
        particle = GetComponent<ParticleSystem>();

        var main = particle.main;
        main.startColor = color;
    }
}
