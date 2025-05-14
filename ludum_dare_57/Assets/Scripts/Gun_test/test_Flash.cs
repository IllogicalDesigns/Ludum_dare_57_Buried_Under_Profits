using System.Collections;
using UnityEngine;

public class test_Flash : MonoBehaviour, IHitReaction, IDeathReaction
{
    public Renderer targetRenderer;
    public float flashDuration = 0.1f;
    private Material originalMaterial;
    public Material flashMaterial;

    private void Awake() {
        if (targetRenderer != null)
            originalMaterial = targetRenderer.material;
    }

    public void React(DamageInstance damageInstance) {
        if (targetRenderer != null && flashMaterial != null)
            StartCoroutine(Flash());
    }

    private IEnumerator Flash() {
        targetRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        targetRenderer.material = originalMaterial;
    }

    public void Die(DamageInstance damageInstance) {
        if (targetRenderer != null && flashMaterial != null)
            StartCoroutine(Flash());
    }
}


