using UnityEngine;

public class Test_gun : MonoBehaviour
{
    public event System.Action OnFireEvent;

    public AudioSource gunShot;

    const float maxDist = float.MaxValue;
    [SerializeField] LayerMask layerMask = ~0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            HandleRay();
            OnFireEvent?.Invoke();
            gunShot.Play();
        }
    }

    private void HandleRay() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDist, layerMask)) {
            HandleHit(hit);
        } else {

        }
    }

    private static void HandleHit(RaycastHit hit) {
        if (hit.collider.TryGetComponent<IDamageable>(out var damageable)) {
            damageable.OnHit(new DamageInstance(1, 1, DamageInstance.DamageType.normal, hit));
        }
        else {
            //Hit like a wall or something
        }
    }
}
