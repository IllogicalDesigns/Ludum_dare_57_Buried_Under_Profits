using UnityEngine;

public class DestroyOnDeathy : MonoBehaviour
{
    public GameObject objToDestroy;

    public void OnDead() {
        if(objToDestroy != null) {
            Destroy(objToDestroy);
        }
        else {
            Destroy(gameObject);
        }
    }
}
