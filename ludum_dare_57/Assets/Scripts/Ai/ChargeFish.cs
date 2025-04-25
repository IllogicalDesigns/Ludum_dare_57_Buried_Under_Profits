using UnityEngine;
using UnityEngine.AI;

public class ChargeFish : MonoBehaviour
{
    public float activationDistance = 15f;
    public float minActivationDistance = 5f;
    Player player;
    Transform attackPoint;
    Health playerHealth;

    public float speed = 5f;


    public int damage = 5;
    public int airDamage = 2;

    public float jitterRange = 1f;
    float jitterX = 0f;
    float jitterY = 0f;
    float jitterZ = 0f;

    [SerializeField] AudioSource start;
    [SerializeField] AudioSource swim;

    public float dotRequirement = 0.5f;
    public LayerMask layerMask = ~0;

    public float distanceToHit = 3f;

    public float cdTime = 2f;
    float cdTimer;

    NavMeshAgent navAgent;

    public enum ChargerState {
        idle,
        navigateToPlayer,
        charging,
        cooldown
    }
    public ChargerState state;
    Threat threat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackPoint = Player.Instance.attackPoint;
        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = false;
        threat = GetComponent<Threat>();

        //Randomly flips the fish
        var scale = transform.localScale;
        scale.x = scale.x * ((Random.value < 0.5f) ? -1 : 1);
        transform.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameManager.GameState.playing) return;

        switch (state) {
            case ChargerState.idle:
                if (AIHelpers.CanThePlayerSeeUs(transform, player.transform, activationDistance, minActivationDistance, dotRequirement, layerMask)) {
                    TransitionToCharging();
                }
                break;
            case ChargerState.navigateToPlayer:
                HandleNavigateToPlayer();
            break;
            case ChargerState.charging:
                HandleCharging();
                break;
            case ChargerState.cooldown:
                Cooldown();
                break;
        }
    }

    private void TransitionToCharging() {
        state = ChargerState.charging;
        navAgent.enabled = false;
        // navAgent.isStopped = true;

        jitterX = Random.Range(-jitterRange, jitterRange);
        jitterY = Random.Range(-jitterRange, jitterRange);
        jitterZ = Random.Range(-jitterRange, jitterRange);

        threat.BecomeThreat();
        start.Play();
        swim.Play();
    }

    private void HandleCharging() {
        bool lineOfSight = Physics.Linecast(transform.position, player.transform.position, layerMask);

       if (lineOfSight)
        {
            TransitionToNavigating();
            return;
        }

        var distance = Vector3.Distance(transform.position, attackPoint.position);

        transform.LookAt(attackPoint.position);

        Vector3 targetPos = attackPoint.position;

        if (distance < (activationDistance * 0.75) && distance > minActivationDistance) {       //For some distance offset our main vector by this rand vector
            targetPos = attackPoint.position + new Vector3(jitterX, jitterY, jitterZ);          //This prevents straight line syndrome
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (distance < distanceToHit) {
            //impact
            playerHealth.SendMessage(Health.OnHitString, new DamageInstance(damage, airDamage));
            threat.UnBecomeThreat();
            swim.Stop();

            TransitionToCooldown();
        }
    }

    private void TransitionToNavigating() {
        state = ChargerState.navigateToPlayer;
        navAgent.enabled = true;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, layerMask)) {
            navAgent.baseOffset = -hit.distance; // Set the base offset to a value that ensures the agent navigates above any obstacles.
        } else {
            navAgent.baseOffset = 1f; // Set a default base offset if no obstacle is found.
        }
    }

    public void HandleNavigateToPlayer(){
            bool lineOfSight = Physics.Linecast(transform.position, player.transform.position, layerMask);
            if(!lineOfSight)
                TransitionToCharging();
            else{
                navAgent.SetDestination(player.transform.position);
            }
    }

    private void TransitionToCooldown() {
        cdTimer = cdTime;
        state = ChargerState.cooldown;
    }

    private void Cooldown() {
        if (cdTimer <= 0)
            TransitionToCharging();
        else
            cdTimer -= Time.deltaTime;

        transform.LookAt(attackPoint);
    }

    public void OnHit(DamageInstance damageInstance) {
        TransitionToCharging();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
