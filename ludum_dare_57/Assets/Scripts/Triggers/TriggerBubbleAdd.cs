using UnityEngine;


public class TriggerBubbleAdd : TriggerBase {
    public AudioClip popSFX;
    public AnimationCurve airProvidedCurve;
    public AnimationCurve difficultyCurve;

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

        Destroy(gameObject);
    }

    public void OnHit(DamageInstance damageInstance) {
        PopBubble();
    }
}
