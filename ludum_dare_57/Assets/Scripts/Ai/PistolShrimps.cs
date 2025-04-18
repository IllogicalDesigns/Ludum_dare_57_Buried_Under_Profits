using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class PistolShrimps : MonoBehaviour {
    public float activationDistance = 15f;
    Player player;
    CharacterController playerController;
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

    Tween dashTween;
    [Space]
    [SerializeField] Vector3 dashOffset = Vector3.up * 1f;
    [SerializeField] float dashDistance = 5f;
    const float PER_OF_CD = 0.5f;
    const float ADJ_RNG = 5f;

    [Space]
    public float projectileSpeed = 20f;
    public bool useGuessAim = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        attackPoint = GameObject.Find("AttackPoint").transform;

        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();
        playerController = player.GetComponent<CharacterController>();
    }

    public void OnRam(Vector3 dir) {
        timer = cooldown;
        isShooting = false;
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.currentGameState != GameManager.GameState.playing) return;

        if (!isShooting && AIHelpers.CanThePlayerSeeUs(transform, player.transform, activationDistance, 0f, dotRequirement, layerMask)) {
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

            var shot = Instantiate(launchedObject, launchPoint.position, Quaternion.identity);
            if (useGuessAim) {
                // Get player position and velocity
                Vector3 playerPos = player.transform.position;
                Vector3 playerVel = playerController.velocity;

                // Calculate direction and distance to player
                Vector3 toPlayer = playerPos - launchPoint.position;
                float bulletSpeed = shot.GetComponent<Bullet>().speed * 2;
                float distance = Vector3.Distance(transform.position, player.transform.position);

                // Calculate time to reach current player position
                float timeToReach = distance / bulletSpeed;

                // Predict future player position
                Vector3 futurePos = playerPos + playerVel * timeToReach;

                // Aim bullet at predicted position
                shot.transform.LookAt(futurePos);

            }
            else {
                shot.transform.LookAt(player.transform.position);
            }

            DashToNewPosition();

            AudioManager.instance.PlaySoundOnPoint(gunShotSFX, transform.position);
            hasPreShotPlayed = false;
        }
    }



    private void DashToNewPosition() {
        Vector3 direction = Random.value < 0.5f ? transform.right : -transform.right;
        var newPosition = transform.position + direction * dashDistance;

        if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, ADJ_RNG, NavMesh.AllAreas)) {
            var blocked = NavMesh.Raycast(transform.position, hit.position, out NavMeshHit navHit, NavMesh.AllAreas);

            if(!blocked)
                dashTween = transform.DOMove(hit.position + dashOffset, cooldown * PER_OF_CD);
            
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

