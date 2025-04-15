using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    public ParticleSystem sandParticleSystem;
    public ParticleSystem sparksParticleSystem;
    public ParticleSystem bloodParticleSystem;
    public ParticleSystem bubbleParticleSystem;
    public ParticleSystem gemsParticleSystem;
    public ParticleSystem goreParticleSystem;

    public enum Particles {
        sand,
        sparks,
        blood,
        bubble,
        gems,
        gore
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(instance == null)
            instance = this;
    }

    public void PlayParticle(Particles chosenParticle, Vector3 position, Vector3 rotation) {
        ParticleSystem particle = null;

        switch (chosenParticle) {
            case Particles.sparks:
                particle = sparksParticleSystem;
                break;
            case Particles.blood:
                particle = bloodParticleSystem;
                break;
            case Particles.bubble:
                particle = bubbleParticleSystem;
                break;
            case Particles.gems:
                particle = gemsParticleSystem;
                break;
            case Particles.gore:
                particle = goreParticleSystem;
                break;
            default:
                particle = sandParticleSystem;
                break;
        }

        if (particle != null) {
            particle.transform.position = position;
            particle.transform.eulerAngles = rotation;
            particle.Play();
        }
    }
}
