using UnityEngine;

public class GemDeath : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] int value = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDead() {
        if(clip != null) AudioSource.PlayClipAtPoint(clip, transform.position);
        GameManager.instance.AddGem(value);
        Destroy(gameObject);
    }
}
