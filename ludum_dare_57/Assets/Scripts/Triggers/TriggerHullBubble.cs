using UnityEngine;


public class TriggerHullBubble : TriggerBase {
    public AudioClip popSFX;
    public AnimationCurve ammoProvidedCurve;
    public Transform ammoBox;
    public MoveToPoint toPoint;
    Transform player;
    public AnimationCurve pickupDistance = new AnimationCurve(new Keyframe(0, 15), new Keyframe(2, 3));

    protected override void Awake() {
        base.Awake();
        player = FindFirstObjectByType<Player>().transform;
    }


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

        var playerHealth = player.GetComponent<Health>();
        playerHealth.Heal(Mathf.RoundToInt(ammoProvidedCurve.Evaluate(playerHealth.hp)));

        FindAnyObjectByType<AmmoCounter>()?.ProvidedAmmo();
        AudioManager.instance.PlaySoundOnPoint(popSFX, transform.position);
        Destroy(gameObject);
    }

    public void OnHit(DamageInstance damageInstance) {
        PopBubble();
    }

    private void Update() {
        if (Vector3.Distance(transform.position, player.position) < pickupDistance.Evaluate(GameManager.instance.difficulty)) {
            PopBubble();
        }
    }
}


