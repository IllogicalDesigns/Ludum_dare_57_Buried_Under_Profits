using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class PlayerGunEffects : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] CameraShake cameraShake;
    [SerializeField] float duration = 0.1f;
    [SerializeField] Vector3 positionPunch = Vector3.one * 0.1f;
    [SerializeField] Vector3 rotationPunch = Vector3.one * 0.1f;

    [Header("Sound effects")]
    [SerializeField] AudioSource gunSfx;
    [SerializeField] AudioSource hitMarkerSfx;
    [SerializeField] AudioClip ammoPickupSfx;
    [SerializeField] AudioClip outOfAmmoSfx;

    [Header("Muzzle flash")]
    [SerializeField] GameObject pointLight;
    [SerializeField] float lightDuration = 0.2f;
    [SerializeField] ParticleSystem gunSparksSystem;

    [Header("Gun Barrel")]
    [SerializeField] Transform gunBarrel;
    [SerializeField] float localZBounce = 0.5f;
    [SerializeField] float bounceDuration = 0.3f;
    float originalGunZ;

    [Header("Hit effects")]
    [SerializeField] GameObject bloodDecal;

    public event System.Action OnHitMarker;

    private void Reset() {
        ammoPickupSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/ammoPickup.mp3");
        outOfAmmoSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/ammoPickup.mp3");
    }

    private void Awake() {
        PlayerGun.GunFiredEvent += PlayGunFireEffects;
        PlayerGun.OutOfAmmoEvent += PlayOutOfAmmoEffects;
        PlayerGun.AmmoAddedEvent += OnAmmoAdd;
        PlayerGun.OnGunHit += OnGunHit;
    }

    private void OnDestroy() {
        PlayerGun.GunFiredEvent -= PlayGunFireEffects;
        PlayerGun.OutOfAmmoEvent -= PlayOutOfAmmoEffects;
        PlayerGun.AmmoAddedEvent -= OnAmmoAdd;
        PlayerGun.OnGunHit -= OnGunHit;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraShake = GetComponent<CameraShake>();
        originalGunZ = gunBarrel.transform.localPosition.y;
    }

    #region AmmoEffects
    private void PlayOutOfAmmoEffects() {
        AudioManager.instance.PlaySoundOnPlayer(outOfAmmoSfx);
    }

    public void OnAmmoAdd() {
        AudioManager.instance.PlaySoundOnPlayer(ammoPickupSfx);
    }
    #endregion

    #region GunShot
    public void HideLight() {
        pointLight.gameObject.SetActive(false);
    }

    private void PlayGunFireEffects() {
        gunSfx.Play();

        gunSparksSystem.Play();

        cameraShake.PunchScreen(duration, positionPunch, rotationPunch);

        //Flash light near muzzle
        pointLight.gameObject.SetActive(true);
        Invoke(nameof(HideLight), lightDuration);

        //Kick gun barrel back
        gunBarrel.DOKill(true);
        gunBarrel.DOLocalMoveY(localZBounce, bounceDuration / 2).SetLoops(2, LoopType.Yoyo);
    }
    #endregion

    #region GunHit
    private void OnGunHit(RaycastHit hit) {
        PlayHitMarkerOnEnemy(hit);
        PlaceDecalOnHit(hit);
        PlayParticleAtHit(hit);
    }

    private void PlayHitMarkerOnEnemy(RaycastHit hit) {
        if (hit.collider.CompareTag("Enemy")) {
            hitMarkerSfx.Play();
            OnHitMarker?.Invoke();
        }
    }

    private void PlaceDecalOnHit(RaycastHit hit) {  //TODO move this to a decal placing manager
        if (hit.collider.CompareTag("Enemy")) {
            var hitDecal = Instantiate(bloodDecal, hit.point, Quaternion.identity) as GameObject; //TODO pool this //Quaternion.EulerAngles(-hit.normal)
            hitDecal.transform.up = hit.normal;
            hitDecal.transform.localScale = new Vector3(Random.Range(0.5f, 1.25f), Random.Range(0.5f, 1.25f), Random.Range(0.5f, 1.25f));
            hitDecal.transform.SetParent(hit.transform);

            var rb = hitDecal.GetComponentInChildren<Rigidbody>();
            var angle = Random.Range(0f, 30f); // Cone angle in degrees
            var randomDir = Random.insideUnitCircle;
            var direction = Quaternion.AngleAxis(angle, hit.normal) * randomDir;
            rb?.AddForce(direction * 5f, ForceMode.Impulse);
            rb?.transform.SetParent(null);
        }
        else {
            //Add non enemy hit decal
        }
    }

    private static void PlayParticleAtHit(RaycastHit hit) { //TODO move this to a ParticleManager placing manager???
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
    #endregion
}
