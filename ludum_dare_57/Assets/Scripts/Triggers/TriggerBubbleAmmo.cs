using UnityEngine;


public class TriggerBubbleAmmo : TriggerBase {
    public AudioClip popSFX;
    public AnimationCurve ammoProvidedCurve;
    public Transform ammoBox;
    public MoveToPoint toPoint;

    protected override void OnTriggerEnter(Collider other) {
        if (!IsCorrectTag(other)) {
            return;
        }

        base.OnTriggerEnter(other);
        PopBubble();
    }

    private void PopBubble() {
        ammoBox.SetParent(null);
        toPoint.enabled = true;

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

