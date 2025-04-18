using UnityEngine;

public class DestroyOnDeathy : MonoBehaviour
{
    public GameObject objToDestroy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDead() {
        if(objToDestroy != null) {
            Destroy(objToDestroy);
        }
        else {
            Destroy(gameObject);
        }
    }
}
