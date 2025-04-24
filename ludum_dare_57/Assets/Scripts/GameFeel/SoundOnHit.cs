using UnityEngine;

public class SoundOnHit : MonoBehaviour
{
    [SerializeField] AudioSource onHit;
    [SerializeField] AudioClip onHitClip;

    [SerializeField] AudioSource onDead;
    [SerializeField] AudioClip onDeadClip;

    public void OnHit(DamageInstance damageInst) {
        if(onHit != null) {
            if(onHitClip != null) onHit.clip = onHitClip;
            onHit.Play();
        } else {
            if (onHitClip != null)
                AudioManager.instance.PlaySoundOnPoint(onHitClip, transform.position);
        }
    }

    public void OnDead() {
        if (onDead != null) {
            if (onDeadClip != null) onDead.clip = onDeadClip;
            onDead.Play();
        }
        else {
            if (onDeadClip != null)
                AudioSource.PlayClipAtPoint(onDeadClip, transform.position);
        }
    }
}
