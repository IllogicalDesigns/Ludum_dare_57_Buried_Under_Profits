using DG.Tweening;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    public static event System.Action OutOfAmmoEvent;
    public static event System.Action GunFiredEvent;
    public static event System.Action AmmoAddedEvent;
    public static event System.Action<RaycastHit> OnGunHit;

    bool isPaused;

    [Header("Damage")]
    public float maxDist = 100f;
    public int baseDamage = 20;
    public int minExtra = 5;
    public int maxExtra = 10;

    public int ammo = 10;

    public float fireRate = 0.2f;
    float nextFireTime;

    [SerializeField] LayerMask layerMask;
    [Space]

    [Header("Recoil")]
    public float upwardsRecoil = -5f;
    public float sidewaysRecoil = 1f;
    public float recoilDuration = 0.1f;
    public float knockbackForce = 3f;
    private CharacterController controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
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
        AmmoAddedEvent?.Invoke();
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
        //PlayOutOfAmmoEffects();
        nextFireTime = Time.time + fireRate;
        OutOfAmmoEvent?.Invoke();
    }

    private void FireGun(Ray ray) {
        GunFiredEvent?.Invoke();

        AddKnockback(); //TODO should this be in the player???

        ammo--;

        nextFireTime = Time.time + fireRate;

        FireRay(ray);
    }

    private void AddKnockback() {
        Vector3 knockbackDirection = -transform.forward; // Opposite to gun's forward
        controller.Move(knockbackDirection * knockbackForce * Time.deltaTime);
    }

    private void FireRay(Ray ray) {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDist, layerMask)) {
            Debug.Log("Hit: " + hit.transform.name);

            //Do recoil
            transform.DOBlendableLocalRotateBy(new Vector3(upwardsRecoil, Random.Range(-sidewaysRecoil, sidewaysRecoil), 0), recoilDuration);

            ApplyDamage(hit);
            OnGunHit?.Invoke(hit);
        }
        else {
            Debug.Log("Missed");
        }
    }

    private void ApplyDamage(RaycastHit hit) {
        var damage = baseDamage + Random.Range(minExtra, maxExtra);
        hit.collider.SendMessage(Health.OnHitString, new DamageInstance(damage, 0), SendMessageOptions.DontRequireReceiver);
    }
}
