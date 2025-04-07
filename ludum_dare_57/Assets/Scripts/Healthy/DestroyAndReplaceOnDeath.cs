using UnityEngine;

public class DestroyAndReplaceOnDeath : MonoBehaviour
{
    [SerializeField] GameObject replacement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDead() {
        Instantiate(replacement, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
