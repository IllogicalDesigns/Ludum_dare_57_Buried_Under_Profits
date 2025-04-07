using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    [SerializeField] GameObject drop;
    [SerializeField] int dropRate = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDead() {
        if(Random.Range(0, 100) < dropRate) {
            Instantiate(drop, transform.position, Quaternion.identity);
        }
    }
}
