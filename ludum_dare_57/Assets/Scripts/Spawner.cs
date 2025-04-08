using System.Collections.Generic;
using UnityEngine;

public class SpawnerWithRaycast : MonoBehaviour {
    [Header("Spawner Settings")]
    public GameObject prefabToSpawn; // The prefab to spawn
    public float spawnHeight = 10f;  // Height at which the object is initially spawned
    public float groundOffset = 0.5f; // Offset above the ground after raycasting
    public float spawnRadius = 50f;  // Radius of the circle for spawning

    [Header("Player Settings")]
    public Transform player;         // Reference to the player transform
    public float minSpawnDistance = 20f; // Minimum distance from the player to spawn

    [Header("Layer Settings")]
    public LayerMask groundLayer;    // Layer mask for detecting ground

    public List<GameObject> spawnedObjects = new List<GameObject>();
    public int desiredSpawnCount = 5;
    public float spawnDelay = 1f;
    public float spawnRate = 1f;
    [Space]
    public Vector3 spawnRotation = Vector3.zero;
    public bool randomRotation;
    public Transform spawnCenter;

    private int maxAttempts = 10; // Prevent infinite loops

    private void Start() {
        if (prefabToSpawn == null || player == null) {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        InvokeRepeating(nameof(AttemptSpawn), spawnDelay, spawnRate);
    }

    private void AttemptSpawn() {
        spawnedObjects.RemoveAll(obj => obj == null);

        if (spawnedObjects.Count < desiredSpawnCount)
            SpawnObject();
    }

    private void SpawnObject() {
        Vector3 randomPosition = Vector3.zero;
        bool validPositionFound = false;

        // Try to find a valid position within maxAttempts
        for (int attempt = 0; attempt < maxAttempts; attempt++) {
            randomPosition = GetRandomPositionInCircle(spawnCenter.position, spawnRadius);
            randomPosition.y = spawnHeight;

            if (Vector3.Distance(randomPosition, player.position) >= minSpawnDistance) {
                validPositionFound = true;
                break;
            }
        }

        if (!validPositionFound) {
            Debug.LogWarning("Failed to find a valid spawn position after maximum attempts.");
            return;
        }

        // Raycast down to find the ground position
        if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, groundLayer)) {
            // Adjust position based on raycast hit point and offset
            randomPosition.y = hitInfo.point.y + groundOffset;

            // Instantiate the prefab at the calculated position
            if (randomRotation) {
                spawnRotation = new Vector3(
                    Random.Range(-180, 180),
                    Random.Range(-180, 180),
                    Random.Range(-180, 180)
                    );
            }

            GameObject spawnedObject = Instantiate(prefabToSpawn, randomPosition, Quaternion.Euler(spawnRotation));
            spawnedObjects.Add(spawnedObject);
        }
        else {
            Debug.LogWarning("Raycast did not hit any ground. Object not spawned.");
        }
    }

    private Vector3 GetRandomPositionInCircle(Vector3 center, float radius) {
        // Generate a random point inside a unit circle and scale it by the radius
        Vector2 randomPoint2D = Random.insideUnitCircle * radius;

        // Convert 2D point to 3D space around the center position
        return new Vector3(center.x + randomPoint2D.x, center.y, center.z + randomPoint2D.y);
    }

    private void OnDrawGizmosSelected() {
        if (player == null) return;

        // Set the color of the Gizmos
        Gizmos.color = Color.green;

        // Draw a wireframe circle to represent the spawn radius
        Gizmos.DrawWireSphere(spawnCenter.position, spawnRadius);

        // Optionally, draw another circle to represent the minimum spawn distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minSpawnDistance);
    }

}
