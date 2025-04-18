using UnityEngine;
using System.Collections.Generic;

public class MineDamage : MonoBehaviour
{
    public float radius = 5f;
    public int damageAmount = 10;
    public int airAmount = 10;
    public List<GameObject> targets = new List<GameObject>();

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
                if (targets.Contains(healthComponent.gameObject)) 
                    continue;

                targets.Add(healthComponent.gameObject);

                Debug.Log("Mine damages " + healthComponent.gameObject.name);

                // Apply damage to the health component
                if (hitCollider.CompareTag("Enemy"))
                    healthComponent.SendMessage("OnHit", new DamageInstance(1000, 1000));
                else
                    healthComponent.SendMessage("OnHit", new DamageInstance(damageAmount, airAmount));
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
