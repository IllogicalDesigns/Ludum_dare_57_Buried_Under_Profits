using UnityEngine;

public class PlayerEffectsOnHit : MonoBehaviour {
    [SerializeField] ParticleSystem Sparks;
    [SerializeField] AudioSource sparkSounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void deaf() {
        AudioListener.volume = 0f;
    }

    public void OnHit(DamageInstance damageInst) {
        Sparks.Play();
        sparkSounds.Play();
    }

    public void OnDead() {
        Sparks.Emit(200);
        Invoke(nameof(deaf), 0.5f);
    }
}

