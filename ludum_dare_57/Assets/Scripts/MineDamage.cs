using UnityEngine;

public class MineDamage : MonoBehaviour
{
    public float radius = 5f;
    public int damageAmount = 10;
    public int airAmount = 10;

    private void Start() {
        ApplyDamage();
        this.enabled = false;
    }

    void ApplyDamage() {
        // Get all colliders within the sphere radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider hitCollider in hitColliders) {
            // Check if the object has a health component
            Health healthComponent = hitCollider.GetComponent<Health>();
            if (healthComponent != null) {
                // Apply damage to the health component
                healthComponent.SendMessage("OnHit", new DamageInstance(damageAmount, damageAmount));
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
