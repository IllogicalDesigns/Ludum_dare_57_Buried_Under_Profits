using UnityEngine;

public class MoveToPoint : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] float speed = 5f;
    [SerializeField] float destroyRadius = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetTransform = GameObject.Find("AttackPoint").transform;
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, targetTransform.position) < destroyRadius) {
            Destroy(targetTransform);
        }
    }
}
