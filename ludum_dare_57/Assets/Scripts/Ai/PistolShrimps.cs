using DG.Tweening;
using UnityEngine;

public class PistolShrimps : MonoBehaviour {
    public float activationDistance = 15f;
    Player player;
    Health playerHealth;
    Transform attackPoint;

    public float cooldown = 10f;
    float timer;

    [SerializeField] Transform launchPoint;
    [SerializeField] Transform launchedObject;

    public int damage = 50;
    public int airDamage = 2;

    public float dotRequirement = -0.65f;
    public LayerMask layerMask = ~0;

    bool isShooting;

    [Space]
    [SerializeField] AudioClip gunShotSFX;
    [SerializeField] float preGunShot = 0.25f;
    [SerializeField] AudioClip preGunShotSfx;
    bool hasPreShotPlayed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        attackPoint = GameObject.Find("AttackPoint").transform;

        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();
    }

    public void OnRam(Vector3 dir) {
        timer = cooldown;
        isShooting = false;
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.currentGameState != GameManager.GameState.playing) return;

        if (!isShooting && AIHelpers.canThePlayerSeeUs(transform, attackPoint, activationDistance, 0f, dotRequirement, layerMask)) {
            StartShooting();
        }

        if (!isShooting) return;

        if (timer <= preGunShot && !hasPreShotPlayed) {
            AudioManager.instance.PlaySoundOnPoint(preGunShotSfx, transform.position);
            hasPreShotPlayed = true;
        }

        if (timer > 0) timer -= Time.deltaTime;
        else if (timer <= 0) {
            transform.LookAt(player.transform);
            //Gun goes off
            timer = cooldown;

            var shot = Instantiate(launchedObject, launchPoint.transform.position, launchPoint.transform.rotation);
            shot.transform.LookAt(player.transform);

            AudioManager.instance.PlaySoundOnPoint(gunShotSFX, transform.position);
            hasPreShotPlayed = false;
        }
    }

    private void StartShooting() {
        if (isShooting) { return; }

        isShooting = true;
        gameObject.SendMessage(Threat.becomeThreatString);
        transform.LookAt(attackPoint);
        hasPreShotPlayed = false;
    }

    public void OnHit(DamageInstance damageInstance) {
        StartShooting();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}

