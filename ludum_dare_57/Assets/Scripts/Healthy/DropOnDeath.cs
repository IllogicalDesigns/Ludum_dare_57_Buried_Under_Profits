using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    [SerializeField] GameObject drop;
    [SerializeField] int dropRate = 100;

    public void OnDead() {
        if(Random.Range(0, 100) < dropRate) {
            Instantiate(drop, transform.position, Quaternion.identity); //TODO pool this
        }
    }
}
