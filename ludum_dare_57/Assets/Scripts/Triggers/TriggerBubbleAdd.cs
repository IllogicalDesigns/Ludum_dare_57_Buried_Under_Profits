using UnityEngine;


public class TriggerBubbleAdd : TriggerBase {
    //public int airProvided = 5;
    public AudioClip popSFX;
    public AnimationCurve airProvidedCurve;

    protected override void OnTriggerEnter(Collider other) {
        if (!IsCorrectTag(other)) {
            return;
        }

        base.OnTriggerEnter(other);

        PopBubble();
    }

    private void PopBubble() {
        GameManager.instance.ProvideAir(Mathf.RoundToInt(airProvidedCurve.Evaluate(GameManager.instance.air)));
        AudioManager.instance.PlaySoundOnPoint(popSFX, transform.position);

        Destroy(gameObject);
    }

    public void OnHit(DamageInstance damageInstance) {
        PopBubble();
    }
}
