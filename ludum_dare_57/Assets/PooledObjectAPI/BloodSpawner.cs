using UnityEngine;
public class BloodSpawner : MonoBehaviour {
    [SerializeField] GameObject bloodPrefab;
    [SerializeField] GameObject decalPrefab;
    private GameObjectPool bloodPool;
    private GameObjectPool decalPool;
    public float force = -1f;

    private void Start() {
        bloodPool = new GameObjectPool(bloodPrefab, 25, 100);
        decalPool = new GameObjectPool(decalPrefab, 25, 100);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                Vector3 spawnPosition = hit.point + hit.normal * 0.1f;

                var spawnedBlood = bloodPool.GetObject(Vector3.zero);
                spawnedBlood.transform.position = spawnPosition;
                spawnedBlood.transform.rotation = Quaternion.LookRotation(hit.normal);
                spawnedBlood.SetActive(true);
                spawnedBlood.transform.SetParent(hit.transform);

                var spawnedDecal = decalPool.GetObject(Vector3.zero);
                spawnedDecal.transform.position = spawnPosition;
                spawnedDecal.transform.rotation = Quaternion.LookRotation(-hit.normal);
                spawnedDecal.SetActive(true);
                spawnedDecal.transform.SetParent(hit.transform);

                if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rigidbody)) {
                    rigidbody.AddForceAtPosition(-hit.normal * force, hit.point, ForceMode.Impulse);
                }
            }
        }
    }

    // Lets a marble tell the spawner it needs to be deleted,
    // without giving marbles access to the pool
    public void RemoveMarble(GameObject marble) {
        bloodPool.ReleaseObject(marble);
    }

    public void RemoveDecal(GameObject marble) {
        decalPool.ReleaseObject(marble);
    }

}