using UnityEngine;

public class DecalDespawn : MonoBehaviour
{
    public BloodSpawner spawner;
    public float lifetime = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawner = FindAnyObjectByType<BloodSpawner>();
    }

    // Update is called once per frame
    void Despawn()
    {
        if (spawner != null) {
            // Call a function on our spawner.
            spawner.RemoveDecal(gameObject);
        }
    }

    private void OnDestroy() {
        transform.SetParent(null);
        Despawn();
    }
}
