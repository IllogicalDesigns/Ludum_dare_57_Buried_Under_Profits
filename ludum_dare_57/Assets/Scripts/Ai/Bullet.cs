using UnityEngine;

public class Bullet : MonoBehaviour {
    public float speed = 10f;
    public int damage = 10;
    public int airDamage = 1;
    bool isPaused;

    void Update() {
        if (isPaused) return;

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetPaused(bool paused) {
        if (paused) {
            Cursor.lockState = CursorLockMode.None;
            isPaused = true;
            Cursor.visible = true;
        }
        else {
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (isPaused) return;
        //if (!other.CompareTag("Player")) return;

        other.SendMessage("OnHit", new DamageInstance(damage, airDamage), SendMessageOptions.DontRequireReceiver);

        Destroy(gameObject);
    }
}