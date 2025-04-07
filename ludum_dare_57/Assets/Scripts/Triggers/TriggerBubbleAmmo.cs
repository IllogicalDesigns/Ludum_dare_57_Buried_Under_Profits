using UnityEngine;


public class TriggerBubbleAmmo : TriggerBase {
    //public int ammoProvided = 5;
    public AudioClip popSFX;
    public AnimationCurve ammoProvidedCurve;

    protected override void OnTriggerEnter(Collider other) {
        if (!IsCorrectTag(other)) {
            return;
        }



        base.OnTriggerEnter(other);
        PopBubble();
    }

    private void PopBubble() {
        var gun = FindAnyObjectByType<PlayerGun>();
        gun?.addAmmo(Mathf.RoundToInt(ammoProvidedCurve.Evaluate(gun.ammo)));
        FindAnyObjectByType<AmmoCounter>()?.ProvidedAmmo();
        AudioManager.instance.PlaySoundOnPoint(popSFX, transform.position);
        Destroy(gameObject);
    }

    public void OnHit(DamageInstance damageInstance) {
        PopBubble();
    }
}

