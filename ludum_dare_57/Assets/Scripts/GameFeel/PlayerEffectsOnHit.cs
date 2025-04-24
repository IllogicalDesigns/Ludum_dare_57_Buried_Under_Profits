using UnityEngine;

public class PlayerEffectsOnHit : MonoBehaviour {
    [SerializeField] ParticleSystem Sparks;
    [SerializeField] AudioSource sparkSounds;

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

