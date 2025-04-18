using UnityEngine;

public class DestroyAndReplaceOnDeath : MonoBehaviour
{
    [SerializeField] GameObject replacement;
    bool isDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDead() {
        if(isDead) return;
        isDead = true;

        Instantiate(replacement, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
