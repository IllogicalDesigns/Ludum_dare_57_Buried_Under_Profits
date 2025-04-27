using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WaveSpawner : MonoBehaviour {
    [Header("Player Settings")]
    public Transform player;         // Reference to the player transform
    public float minSpawnDistance = 20f; // Minimum distance from the player to spawn

    public List<GameObject> spawnedObjects = new List<GameObject>();

    [Space]
    public Vector3 spawnRotation = Vector3.zero;
    public Transform spawnCenter;

    private int maxAttempts = 10; // Prevent infinite loops

    public List<Wave> waves = new List<Wave>();
    public List<GameObject> currentlySpawned = new List<GameObject>();

    public int currentWave = 0;

    public UnityEvent onSpawningStart;
    public UnityEvent onSpawningEnd;

    public Vector3 spawnSize = Vector3.one;

    private void Start() {
        if (player == null) {
            Debug.LogError("Please assign all required references in the inspector.");
            return;
        }

        onSpawningStart.Invoke();
    }

    public void SpawnNextWave(){
        spawnedObjects.RemoveAll(obj => obj == null);
        Debug.Log("Spawning wave " + currentWave + " with ");

        if(currentWave <= waves.Count-1) {
            Wave curWave = waves[currentWave];  //Get the current wave
            for (int spawnI = currentWave; spawnI < curWave.spawnObjects.Count; spawnI++)  //crawl through each spawn object in the current wave
            {
                SpawnObject objectInWave = curWave.spawnObjects[spawnI];
                for (int i= 0; i < objectInWave.amountToSpawn; i++) {  //Spawn the requested amount of objects in the current wave
                    Debug.Log(objectInWave.prefabToSpawn.name + " " + objectInWave.amountToSpawn);
                    SpawnOnNavMesh(objectInWave.prefabToSpawn);
                }
            }
        }

        if(currentWave >= waves.Count-1 && currentlySpawned.Count == 0){
            onSpawningEnd.Invoke();  //We are out of waves
            enabled = false;
        }

        currentWave++;
    }

    private void Update() {
        currentlySpawned.RemoveAll(obj => obj == null);

        if(currentlySpawned.Count == 0){
            SpawnNextWave();
        }

        if(Input.GetKeyDown(KeyCode.KeypadDivide)){
            foreach(GameObject obj in currentlySpawned){
                Destroy(obj);
            }
        }
    }

    private void SpawnOnNavMesh(GameObject prefabToSpawn){
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

        GameObject spawnedObject = Instantiate(prefabToSpawn, randomPosition, Quaternion.Euler(spawnRotation)); //TODO pool this
        spawnedObjects.Add(spawnedObject);
        currentlySpawned.Add(spawnedObject);
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

    private void OnDrawGizmosSelected() {
        if (player == null) return;

        // Optionally, draw another circle to represent the minimum spawn distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minSpawnDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnCenter.position, spawnSize);
    }

}

[System.Serializable]
public class Wave {
    public List<SpawnObject> spawnObjects = new List<SpawnObject>();
}

[System.Serializable]
public class SpawnObject {
    public GameObject prefabToSpawn;
    public int amountToSpawn = 1;
}


