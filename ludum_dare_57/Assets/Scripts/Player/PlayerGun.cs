using DG.Tweening;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    bool isPaused;
    public float maxDist = 100f;
    public int baseDamage = 20;
    public int minExtra = 5;
    public int maxExtra = 10;

    public int ammo = 10;

    [SerializeField] AudioSource gunSfx;
    [SerializeField] AudioSource hitMarkerSfx;
    [SerializeField] AudioClip ammoPickupSfx;
    [SerializeField] AudioClip outOfAmmoSfx;
    [SerializeField] CameraShake cameraShake;

    [SerializeField] float duration = 0.1f;
    [SerializeField] Vector3 positionPunch = Vector3.one;
    [SerializeField] Vector3 rotationPunch = Vector3.one * 15f;

    [SerializeField] ParticleSystem bloodSystem;
    [SerializeField] ParticleSystem sparksSystem;
    [SerializeField] GameObject pointLight;

    [SerializeField] Transform gunBarrel;
    [SerializeField] float localZBounce;
    float origZ;
    [SerializeField] float bounceDuration = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraShake = GetComponent<CameraShake>();
        origZ = gunBarrel.transform.localPosition.y;
    }

    public void SetPaused(bool paused) {
        if (paused) {
            isPaused = true;
        }
        else {
            isPaused = false;
        }
    }

    public void addAmmo(int value) {
        ammo += value;
        AudioManager.instance.PlaySoundOnPlayer(ammoPickupSfx);
    }

    public void HideLight() {
        pointLight.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isPaused) { return; }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0) && ammo > 0) {
            gunSfx.Play();
            sparksSystem.Play();
            cameraShake.PunchScreen(duration, positionPunch, rotationPunch);
            ammo--;

            if (Physics.Raycast(ray, out hit, maxDist)) {
                Debug.Log("Hit: " + hit.transform.name);
                var damage = baseDamage + Random.Range(minExtra, maxExtra);
                hit.collider.SendMessage(Health.OnHitString, new DamageInstance(damage, 0), SendMessageOptions.DontRequireReceiver);

                pointLight.gameObject.SetActive(true);
                Invoke(nameof(HideLight), 0.1f);

                gunBarrel.DOKill(true);
                gunBarrel.DOLocalMoveY(localZBounce, bounceDuration/2).SetLoops(2, LoopType.Yoyo);

                if (hit.collider.TryGetComponent<Health>(out Health hp)) {
                    hitMarkerSfx.Play();
                }

                if (hit.collider.TryGetComponent<OnHitSpawnParticle>(out OnHitSpawnParticle spawnParticle)) {
                    spawnParticle.particleSystem.transform.position = hit.point;
                    spawnParticle.particleSystem.transform.forward = hit.normal;
                    spawnParticle.particleSystem.Play();
                } else {
                    bloodSystem.transform.position = hit.point;
                    bloodSystem.transform.forward = hit.normal;
                    bloodSystem.Play();
                }
            }
            else {
                Debug.Log("Missed");
            }
        }
        else if(Input.GetMouseButtonDown(0) && ammo <= 0) {
            AudioManager.instance.PlaySoundOnPlayer(outOfAmmoSfx);
        }
    }
}
