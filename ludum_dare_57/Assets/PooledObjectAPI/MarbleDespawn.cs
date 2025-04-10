using UnityEngine;

public class MarbleDespawn : MonoBehaviour {
    public BloodSpawner spawner;  // The marble spawner.
    // Start is called before the first frame update
    void Start() {
        // Find our marble spawner.
        // This will get weird if you try to make more than one spawner.
        spawner = FindAnyObjectByType<BloodSpawner>();
    }

    public void OnParticleSystemStopped() {
        if (spawner != null) {
            // Call a function on our spawner.
            spawner.RemoveMarble(gameObject);
        }
    }
}