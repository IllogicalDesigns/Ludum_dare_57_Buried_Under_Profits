using UnityEngine;

public class GemDeath : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] int value = 1;

    public void OnDead() {
        if(clip != null) AudioSource.PlayClipAtPoint(clip, transform.position);
        GameManager.instance.AddGem(value);
        Destroy(gameObject);
    }
}
