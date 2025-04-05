using System.Collections;
using UnityEngine;
using DG.Tweening;

public class EnemyHitEffect : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    Material originalMat;
    Material flashMat;

    [SerializeField] Vector3 punchScale = Vector3.one * 0.5f;
    [SerializeField] float duration = 0.1f;

    private void Reset() {
        renderer = gameObject.GetComponent<Renderer>();
    }

    void Start() {
        originalMat = renderer.material;
        flashMat = Resources.Load<Material>("FlashMaterial"); // Load your flash material
    }

    public void OnHit(DamageInstance damageInstance) {
        StartCoroutine(Flash());
    }

    IEnumerator Flash() {
        yield return null; // Wait for next frame

        renderer.material = flashMat;

        transform.DOPunchScale(punchScale, duration);

        yield return null; // Wait for another frame

        renderer.material = originalMat;
    }
}
