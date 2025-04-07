using System.Collections.Generic;
using UnityEngine;

public class SpawnerWithRaycast : MonoBehaviour {
    [Header("Spawner Settings")]
    public GameObject prefabToSpawn; // The prefab to spawn
    public BoxCollider spawnCube;    // The box collider defining the spawn area
    public float spawnHeight = 10f;  // Height at which the object is initially spawned
    public float groundOffset = 0.5f; // Offset above the ground after raycasting

    [Header("Player Settings")]
    public Transform player;         // Reference to the player transform
    public float spawnDistance = 50f; // Minimum distance from the player to spawn

    [Header("Layer Settings")]
    public LayerMask groundLayer;    // Layer mask for detecting ground

    public List<GameObject> spawnedObjects = new List<GameObject>();
    public int desiredSpawnCount = 5;
    public float spawnDelay = 1f;
    public float spawnRate = 1f;
    [Space]
    public Vector3 spawnRotation = Vector3.zero;

    private void Start() {
        if (prefabToSpawn == null || spawnCube == null || player == null) {
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
        // Get a random position within the bounds of the BoxCollider
        Vector3 randomPosition = GetRandomPositionInBox(spawnCube);

        // Adjust the position to be at the specified height
        randomPosition.y = spawnHeight;

        // Ensure the spawn position is out of sight of the player (distance check)
        while (Vector3.Distance(randomPosition, player.position) < spawnDistance) {
            randomPosition = GetRandomPositionInBox(spawnCube);
            randomPosition.y = spawnHeight;
        }

        // Raycast down to find the ground position
        if (Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, groundLayer)) {
            // Adjust position based on raycast hit point and offset
            randomPosition.y = hitInfo.point.y + groundOffset;

            // Instantiate the prefab at the calculated position
            GameObject spawnedObject = Instantiate(prefabToSpawn, randomPosition, Quaternion.EulerAngles(spawnRotation));
            spawnedObjects.Add(spawnedObject);
            //Debug.Log($"Spawned {spawnedObject.name} at {randomPosition}");
        }
        else {
            Debug.LogWarning("Raycast did not hit any ground. Object not spawned.");
        }
    }

    private Vector3 GetRandomPositionInBox(BoxCollider boxCollider) {
        // Get the bounds of the BoxCollider in world space
        Bounds bounds = boxCollider.bounds;

        // Generate a random position within the bounds
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, randomY, randomZ);
    }


}
