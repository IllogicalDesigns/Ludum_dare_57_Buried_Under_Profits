using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public Vector3 spawnSize = Vector3.one * 15;

    private int maxAttempts = 10; // Prevent infinite loops

    private void Start() {
        player = FindAnyObjectByType<Player>().transform;

        if (prefabToSpawn == null || player == null) {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        InvokeRepeating(nameof(AttemptSpawn), spawnDelay, spawnRate);
    }

    private void AttemptSpawn() {
        spawnedObjects.RemoveAll(obj => obj == null);

        if (spawnedObjects.Count < desiredSpawnCount)
            SpawnOnNavMesh(prefabToSpawn);
    }

    private void SpawnOnNavMesh(GameObject _prefabToSpawn){
        var randomPosition = GetRandomPositionInsideBox(spawnCenter.position, spawnSize);

        // Try to find a valid position within maxAttempts
        for (int attempt = 0; attempt < maxAttempts; attempt++) {
            if (Vector3.Distance(randomPosition, player.position) >= minSpawnDistance) {
                break;
            }

            randomPosition = GetRandomPositionInsideBox(spawnCenter.position, spawnSize);
        }

        if(NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 20f, NavMesh.AllAreas))
            randomPosition = hit.position;

        randomPosition.y += groundOffset;

        GameObject spawnedObject = Instantiate(_prefabToSpawn, randomPosition, Quaternion.Euler(spawnRotation)); //TODO pool this
        spawnedObjects.Add(spawnedObject);
    }

    Vector3 GetRandomPositionInsideBox(Vector3 center, Vector3 size)
    {
        // Calculate half of the size to determine the bounds
        Vector3 halfSize = size / 2f;

        // Generate a random position within the bounds of the box
        Vector3 randomPosition = new Vector3(
            Random.Range(-halfSize.x, halfSize.x),
            Random.Range(-halfSize.y, halfSize.y),
            Random.Range(-halfSize.z, halfSize.z)
        );

        // Add the center position to the random position
        return center + randomPosition;
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
