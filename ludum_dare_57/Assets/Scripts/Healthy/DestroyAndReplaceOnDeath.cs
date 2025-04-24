using UnityEngine;

public class DestroyAndReplaceOnDeath : MonoBehaviour
{
    [SerializeField] GameObject replacement;
    bool isDead;

    public void OnDead() {
        if(isDead) return;
        isDead = true;

        Instantiate(replacement, transform.position, transform.rotation); //TODO pool this
        Destroy(gameObject);
    }
}
