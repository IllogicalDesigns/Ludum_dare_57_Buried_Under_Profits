using System.Collections;
using UnityEngine;
using DG.Tweening;

public class test_Size : MonoBehaviour, IHitReaction, IDeathReaction {
    public float punchDuration = 0.1f;
    public Vector3 punchSize = Vector3.one * 0.1f;
    Vector3 startSize;

    private void Awake() {
        startSize = transform.localScale;
    }

    public void React(DamageInstance damageInstance) {
        PunchScale();
    }

    public void Die(DamageInstance damageInstance) {
        PunchScale();
    }

    private void PunchScale() {
        transform.DOKill(true);
        transform.localScale = startSize;
        transform.DOPunchScale(punchSize, punchDuration);
    }
}



