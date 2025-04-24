using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class PistolShrimps : MonoBehaviour {
    public float activationDistance = 15f;

    public enum ShrimpState {
        idle,
        preShot,
        shooting,
        dodge,
        cooldown,
    }
    public ShrimpState state;

    public float preShotTime = 2f;
    float preShotTimer;

    public float cdTime = 2f;
    float cdTimer;

    Player player;
    CharacterController playerController;

    Transform attackPoint;

    public float cooldown = 10f;


    [SerializeField] Transform launchPoint;
    [SerializeField] Transform launchedObject;

    public int damage = 50;
    public int airDamage = 2;

    public float dotRequirement = -0.65f;
    public LayerMask layerMask = ~0;

    [Space]
    [SerializeField] AudioClip gunShotSFX;
    [SerializeField] AudioClip preGunShotSfx;

    Tween dashTween;
    [Space]
    [SerializeField] Vector3 dashOffset = Vector3.up * 1f;
    [SerializeField] float dashDistance = 5f;

    const float PERCENT_OF_CD = 0.5f;
    const float AGENT_ADJ_RANGE = 5f;

    [Space]
    public float projectileSpeed = 20f;
    public bool useGuessAim = true;
    Threat threat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        attackPoint = Player.Instance.attackPoint;

        player = FindAnyObjectByType<Player>();
        playerController = player.GetComponent<CharacterController>();
        threat = GetComponent<Threat>();

    }

    public void OnRam(Vector3 dir) {
        if (state == ShrimpState.idle)
            TransitionToPreShot();
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.currentGameState != GameManager.GameState.playing) return;

        switch (state) {
            case ShrimpState.idle:
                if (AIHelpers.CanThePlayerSeeUs(transform, player.transform, activationDistance, 0f, dotRequirement, layerMask)) 
                    TransitionToPreShot();
                break;
            case ShrimpState.preShot:
                PreShot();  //Just look at players
                break;
            case ShrimpState.shooting:
                Shooting();
                break;
            case ShrimpState.dodge:
                break;
            case ShrimpState.cooldown:
                Cooldown();
                break;
        }
    }

    private Vector3 GetDashPosition() {
        Vector3 direction = Random.value < 0.5f ? transform.right : -transform.right;
        var newPosition = transform.position + direction * dashDistance;

        if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit, AGENT_ADJ_RANGE, NavMesh.AllAreas)) {
            var blocked = NavMesh.Raycast(transform.position, hit.position, out NavMeshHit navHit, NavMesh.AllAreas);

            if (!blocked)
                return (hit.position + dashOffset);
        }
        
        //Give garbage, to filter out later
        return Vector3.zero;
    }

    void TransitionToPreShot() {
        AudioManager.instance.PlaySoundOnPoint(preGunShotSfx, transform.position);
        threat.BecomeThreat();
        state = ShrimpState.preShot;
        preShotTimer = preShotTime;
    }

    void PreShot() {
        if(preShotTimer <= 0) {
            TransitionToShooting();
        } else 
            preShotTimer -= Time.deltaTime;

        transform.LookAt(attackPoint);
    }

    private void TransitionToShooting() {
        state = ShrimpState.shooting;
    }

    private void Shooting() {
        transform.LookAt(player.transform);

        AudioManager.instance.PlaySoundOnPoint(gunShotSFX, transform.position);

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

        TransitionToDodging();
    }

    void TransitionToDodging() {
        state = ShrimpState.dodge;

        var dashPos = GetDashPosition();

        if(dashPos != Vector3.zero)  //If its vector3.zero its likely because its inside of a wall, dont dodge, simply dont dodge
            dashTween = transform.DOMove(dashPos, cooldown * PERCENT_OF_CD).OnComplete(() => { TransitionToCooldown(); });
        else
            TransitionToCooldown();
    }

    void TransitionToCooldown() {
        state = ShrimpState.cooldown;
        cdTimer = cdTime;
    }

    private void Cooldown() {
        if (cdTimer <= 0)
            TransitionToPreShot();
        else
            cdTimer -= Time.deltaTime;

        transform.LookAt(attackPoint);
    }

    public void OnHit(DamageInstance damageInstance) {
        if(state == ShrimpState.idle) 
            TransitionToPreShot();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}

