using UnityEngine;


public class TriggerBubbleAdd : TriggerBase {
    public AudioClip popSFX;
    public AnimationCurve airProvidedCurve;
    public AnimationCurve difficultyCurve;
    Transform player;
    public AnimationCurve pickupDistance = new AnimationCurve(new Keyframe(0, 15), new Keyframe(2, 3));

    public GameObject bubblePop;

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
        var diffMultii = difficultyCurve.Evaluate(GameManager.instance.difficulty);
        var airAmount = airProvidedCurve.Evaluate(GameManager.instance.air);

        var providedAir = Mathf.RoundToInt(airAmount * diffMultii);
        if (providedAir <= 0) providedAir = 1;

        GameManager.instance.ProvideAir(providedAir);
        AudioManager.instance.PlaySoundOnPoint(popSFX, transform.position);

        Instantiate(bubblePop, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    public void OnHit(DamageInstance damageInstance) {
        PopBubble();
    }

    private void Update() {
        if(Vector3.Distance(transform.position, player.position) < pickupDistance.Evaluate(GameManager.instance.difficulty)) {
            PopBubble();
        }
    }
}
