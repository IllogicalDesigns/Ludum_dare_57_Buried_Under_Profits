using UnityEngine;
using DG.Tweening;
using System;

public class GoreTest : MonoBehaviour
{
    public float forceMultipler = -2f;
    public ParticleSystem bloodParticles;

    public Transform goreTestSubject;
    public Transform moveTarget;
    public Transform lookTarget;
    public float duration = 10f;

    public LayerMask gore;
    public LayerMask health;

    public int damage = 34;

    public Health goreTestHealth;

    float nextTime;
    public float fireRate = 0.1f;
    float fireTimer;

    public AudioClip gunShot;

    public Transform camTrans;
    public float punchScale = 0.15f;
    public float punchDuration = 0.1f;

    public GameObject decal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goreTestHealth.OnDeathEvent += HandleDeath;
        camTrans = Camera.main.transform;
    }

    private void HandleDeath() {
        var cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in cols) {
            col.enabled = false;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, health)) {
            if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
                rb.AddForceAtPosition(hit.normal * forceMultipler, hit.point, ForceMode.Impulse);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //goreTestSubject.DOLookAt(lookTarget.position, duration);
        //goreTestSubject.DOMove(moveTarget.position, duration);

        if (Input.GetMouseButton(0) && fireTimer <= 0) {
            fireTimer = fireRate;

            if (gunShot) AudioSource.PlayClipAtPoint(gunShot, transform.position);

            if (camTrans) {
                camTrans.DOPunchPosition(Vector3.one * punchScale, punchDuration);
                camTrans.DOPunchRotation(Vector3.one * punchScale, punchDuration);
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, gore)) {
                if(hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
                    rb.isKinematic = false;
                    rb.AddForceAtPosition(hit.normal * forceMultipler, hit.point, ForceMode.Impulse);

                    rb.transform.SetParent(null);

                    bloodParticles.transform.position = hit.point;
                    bloodParticles.transform.forward = hit.normal;
                    bloodParticles.Play();
                }
            }

            if (Physics.Raycast(ray, out RaycastHit hit1, Mathf.Infinity, health)) {
                if (hit1.collider.TryGetComponent<Health>(out Health hp)) {
                    hp.OnHit(new DamageInstance(damage, 0));

                    var decObj = Instantiate(decal, hit1.point, Quaternion.identity) as GameObject;
                    decObj.transform.forward = -hit1.normal;
                    decObj.transform.SetParent(hp.transform);
                }
            }

            nextTime += fireRate;
        }

        fireTimer -= Time.deltaTime;
    }
}
