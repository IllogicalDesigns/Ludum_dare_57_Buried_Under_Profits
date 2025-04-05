using UnityEngine;

public class SoundOnHit : MonoBehaviour
{
    [SerializeField] AudioSource onHit;
    [SerializeField] AudioClip onHitClip;

    [SerializeField] AudioSource onDead;
    [SerializeField] AudioClip onDeadClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHit(DamageInstance damageInst) {
        if(onHit != null) {
            if(onHitClip != null) onHit.clip = onHitClip;
            onHit.Play();
        } else {
            if (onHitClip != null)
                AudioSource.PlayClipAtPoint(onHitClip, transform.position);
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
