using UnityEngine;

public class Bullet : MonoBehaviour {
    public float speed = 10f;
    public int damage = 10;
    public int airDamage = 1;

    void Update() {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        //if (!other.CompareTag("Player")) return;

        other.SendMessage("OnHit", new DamageInstance(damage, airDamage), SendMessageOptions.DontRequireReceiver);

        Destroy(gameObject);
    }
}