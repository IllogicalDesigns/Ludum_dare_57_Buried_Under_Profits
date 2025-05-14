using UnityEngine;

public class MoveToPoint : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] float speed = 5f;
    [SerializeField] float destroyRadius = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Player.Instance != null)
            targetTransform = Player.Instance.attackPoint;
    }

    void Update() {
        if(targetTransform == null) { enabled = false; return; }

        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, targetTransform.position) < destroyRadius) {
            Destroy(gameObject);
        }
    }
}
