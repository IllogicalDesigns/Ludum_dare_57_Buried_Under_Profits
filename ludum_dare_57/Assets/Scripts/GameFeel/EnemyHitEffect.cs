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
    Rigidbody rb;

    private void Reset() {
        renderer = gameObject.GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    void Start() {
        if(TryGetComponent<Health>(out Health health)) {
            
        }

        if(renderer) originalMat = renderer.material;
        flashMat = Resources.Load<Material>("FlashMaterial"); // Load your flash material
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public void OnHit(DamageInstance damageInstance) {
        StartCoroutine(Flash());
    }

    public void OnRam(Vector3 dir) {
        rb.isKinematic = false;
        rb.AddForce(dir, ForceMode.Impulse);

        Invoke(nameof(ReEnableRigidBody), 1f);
    }

    void ReEnableRigidBody() {
        rb.isKinematic = true;
    }

    IEnumerator Flash() {
        yield return null; // Wait for next frame

        if (renderer) renderer.material = flashMat;

        transform.DOPunchScale(punchScale, duration);

        yield return null; // Wait for another frame

        if (renderer) renderer.material = originalMat;
    }
}
