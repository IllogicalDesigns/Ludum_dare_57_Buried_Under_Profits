using UnityEngine;

public class Health : MonoBehaviour
{
    public const string OnHitString = "OnHit";
    public const string OnDeadString = "OnDead";
    public int hp = 1;
    public int maxHp = 1;


    public void OnHit(int damage) {
        hp -= damage;

        hp = Mathf.Clamp(hp, 0, maxHp);

        if(hp <= 0) {
            gameObject.SendMessage(OnDeadString);
        }
    }
}
