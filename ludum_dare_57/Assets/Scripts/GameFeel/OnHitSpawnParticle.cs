using UnityEngine;

public class OnHitSpawnParticle : MonoBehaviour
{
    public string particleNameFinder = "";
    public ParticleSystem particleSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (particleSystem == null) {
            var gameObject = GameObject.Find(particleNameFinder);
            if (gameObject == null) {
                Debug.Log("Null particleNameFinder: " + particleNameFinder);
                return;
            }

            particleSystem = gameObject.GetComponent<ParticleSystem>();
            if (particleSystem == null) {
                Debug.Log("Not particle system attatched to : " + particleNameFinder);
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
