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

    [SerializeField] GameObject bloodDecal;
    [SerializeField] ParticleSystem gunSparksSystem;
    [SerializeField] GameObject pointLight;

    [SerializeField] Transform gunBarrel;
    [SerializeField] float localZBounce;
    float origZ;
    [SerializeField] float bounceDuration = 0.2f;

    public float lightDuration = 0.2f;

    public float fireRate = 0.2f;
    float nextFireTime;

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

        bool waitedForFireRate = Time.time > nextFireTime;
        if(Input.GetMouseButtonDown(0) && ammo > 0) {
            FireGun(ray);
        }
        else if (Input.GetMouseButton(0) && waitedForFireRate && ammo > 0) {
            FireGun(ray);
        }
        else if(Input.GetMouseButton(0) && waitedForFireRate && ammo <= 0) {
            OutOfAmmo();
        }
        else if (Input.GetMouseButtonDown(0) && waitedForFireRate && ammo <= 0) {
            OutOfAmmo();
        }
    }

    private void OutOfAmmo() {
        PlayOutOfAmmoEffects();
        nextFireTime = Time.time + fireRate;
    }

    private void FireGun(Ray ray) {
        PlayGunFireEffects();
        ammo--;
        nextFireTime = Time.time + fireRate;
        FireRayAndHandleEffects(ray);
    }

    private void FireRayAndHandleEffects(Ray ray) {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDist)) {
            Debug.Log("Hit: " + hit.transform.name);

            ApplyDamage(hit);
            PlayHitMarkerOnEnemy(hit);
            PlaceDecalOnHit(hit);
            PlayParticleAtHit(hit);
        }
        else {
            Debug.Log("Missed");
        }
    }

    private void PlayOutOfAmmoEffects() {
        AudioManager.instance.PlaySoundOnPlayer(outOfAmmoSfx);
    }

    private void ApplyDamage(RaycastHit hit) {
        var damage = baseDamage + Random.Range(minExtra, maxExtra);
        hit.collider.SendMessage(Health.OnHitString, new DamageInstance(damage, 0), SendMessageOptions.DontRequireReceiver);
    }

    private void PlayHitMarkerOnEnemy(RaycastHit hit) {
        if (hit.collider.TryGetComponent<Health>(out Health hp)) {
            hitMarkerSfx.Play();
        }
    }

    private void PlaceDecalOnHit(RaycastHit hit) {
        if (hit.collider.CompareTag("Enemy")) {
            var hitDecal = Instantiate(bloodDecal, hit.point, Quaternion.EulerAngles(-hit.normal)) as GameObject;
            hitDecal.transform.SetParent(hit.transform);
        }
        else {
            //Add non enemy hit decal
        }
    }

    private void PlayGunFireEffects() {
        gunSfx.Play();
        gunSparksSystem.Play();
        cameraShake.PunchScreen(duration, positionPunch, rotationPunch);

        pointLight.gameObject.SetActive(true);
        Invoke(nameof(HideLight), lightDuration);

        gunBarrel.DOKill(true);
        gunBarrel.DOLocalMoveY(localZBounce, bounceDuration / 2).SetLoops(2, LoopType.Yoyo);
    }

    private static void PlayParticleAtHit(RaycastHit hit) {
        if (hit.collider.TryGetComponent<OnHitSpawnParticle>(out OnHitSpawnParticle spawnParticle)) {
            ParticleManager.instance?.PlayParticle(spawnParticle.typeOfParticles, hit.point, hit.normal);
            //spawnParticle.particleSystem.transform.position = hit.point;
            //spawnParticle.particleSystem.transform.forward = hit.normal;
            //spawnParticle.particleSystem.Play();
        }
        else {
            ParticleManager.instance?.PlayParticle(ParticleManager.Particles.sand, hit.point, hit.normal);
            //sandSystem.transform.position = hit.point;
            //sandSystem.transform.forward = hit.normal;
            //sandSystem.Play();
        }
    }
}
