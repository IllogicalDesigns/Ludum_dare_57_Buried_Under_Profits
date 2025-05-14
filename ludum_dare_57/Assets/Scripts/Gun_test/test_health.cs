using UnityEngine;

public class test_health : MonoBehaviour, IDamageable {
    public int hp = 100;
    public event System.Action OnHitEvent;
    public event System.Action OnDeathEvent;

    private bool isDead;

    private IHitReaction[] hitReactions;
    private IDeathReaction[] deathReactions;

    private void Awake() {
        hitReactions = GetComponents<IHitReaction>();
        deathReactions = GetComponents<IDeathReaction>();
    }

    public void OnHit(DamageInstance damageInstance) {
        hp -= damageInstance.damage;

        //call all reactions
        foreach (var reaction in hitReactions)
            reaction.React(damageInstance);

        OnHitEvent?.Invoke();

        if (hp <= 0)
            Kill(damageInstance);
    }

    private void Kill(DamageInstance damageInstance) {
        if (isDead) return;

        isDead = true;

        //call all reactions
        foreach (var reaction in deathReactions)
            reaction.Die(damageInstance);

        OnDeathEvent?.Invoke();
    }
}

public interface IDamageable {
    void OnHit(DamageInstance damageInstance);
}

public interface IHitReaction {
    void React(DamageInstance damageInstance);
}

public interface IDeathReaction {
    void Die(DamageInstance damageInstance);
}
