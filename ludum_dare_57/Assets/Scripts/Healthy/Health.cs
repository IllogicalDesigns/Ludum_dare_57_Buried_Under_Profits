using UnityEngine;

public class Health : MonoBehaviour
{
    public static event System.Action OnHitEvent;
    public static event System.Action OnHealEvent;

    public const string OnHitString = "OnHit";
    public const string OnDeadString = "OnDead";
    public int hp = 1;
    public int maxHp = 1;
    public bool canTakeDamage = true;


    public void OnHit(DamageInstance damageInstance) {
        if (!canTakeDamage) return;

        OnHitEvent?.Invoke();

        hp -= damageInstance.damage;

        hp = Mathf.Clamp(hp, 0, maxHp);

        if(hp <= 0) {
            gameObject.SendMessage(OnDeadString);
        }
    }

    public void Heal(int value){
        hp += value;
        hp = Mathf.Clamp(hp, 0, maxHp);
        OnHealEvent?.Invoke();
    }
}

public class DamageInstance {
    public int damage;
    public int airDamage;
    public DamageType damageType;

    public DamageInstance(int damage, int airDamage, DamageType damageType = DamageType.normal) {
        this.damage = damage;
        this.airDamage = airDamage;
        this.damageType = damageType;
    }

    public enum DamageType {
        normal,
        collision,
        laser,
        explosion
    }
}
