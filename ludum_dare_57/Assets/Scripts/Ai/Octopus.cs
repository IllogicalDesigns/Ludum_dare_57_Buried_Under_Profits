using UnityEngine;
using UnityEngine.AI;

public class Octopus : MonoBehaviour
{
    public float activationDistance = 15f;
    public float minActivationDistance = 5f;
    Player player;
    Transform attackPoint;
    Health playerHealth;

    public float speed = 5f;


    public int damage = 1;
    public int airDamage = 1;

    public float timeBetweenTicks = 1f;
    float tickTimer;

    [SerializeField] AudioSource start;
    [SerializeField] AudioSource swim;

    public float dotRequirement = 0.5f;
    public LayerMask layerMask = ~0;

    public float distanceToLatch = 3f;

    public float latchTime = 2f;
    float latchTimer;

    public float cdTime = 2f;
    float cdTimer;

    NavMeshAgent navAgent;

    Rigidbody rigid;
    public float throwForceMulti = 1f;

    public enum ChargerState {
        idle,
        navigateToPlayer,
        charging,
        latched,
        cooldown
    }
    public ChargerState state;

    public Material normalMaterial;
    public Material latchedMaterial;
    public MeshRenderer meshRenderer;

    Threat threat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackPoint = Player.Instance.attackPoint;
        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.enabled = false;
        rigid = GetComponent<Rigidbody>();
        threat = GetComponent<Threat>();

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
            case ChargerState.latched:
                HandleLatching();
                break;
            case ChargerState.cooldown:
                Cooldown();
                break;
        }
    }

    private void TransitionToCharging() {
        state = ChargerState.charging;
        navAgent.enabled = false;

        threat.BecomeThreat();

        if(start != null) start.Play();
        if(swim != null) swim.Play();
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

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        //TODO make sure we are in the front view cone of the player and navigate to this

        if (distance < distanceToLatch && !player.isDodging) {
            //impact
            // playerHealth.SendMessage(Health.OnHitString, new DamageInstance(damage, airDamage));
            transform.SetParent(player.transform);
            swim.Stop();

            TransitionToLatched();
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

    private void TransitionToLatched() {
        state = ChargerState.latched;
        latchTimer = latchTime;
        if(swim != null)
            swim.Stop();

        tickTimer = 0f;

        GetComponent<Health>().canTakeDamage = false;

        meshRenderer.material = latchedMaterial;
    }

    private void HandleLatching() {
        float mouseSpeedX = Input.GetAxis("Mouse X") / Time.deltaTime;
        float mouseSpeedY = Input.GetAxis("Mouse Y") / Time.deltaTime;
        float mouseSpeed = Mathf.Sqrt(mouseSpeedX * mouseSpeedX + mouseSpeedY * mouseSpeedY);

        //as mouse is moved, the timer goes down, at zero we transition to cooldown
        if(mouseSpeed > 0)
            latchTimer -= Time.deltaTime * mouseSpeed;

        if(latchTimer <= 0){   
            transform.SetParent(null);

            Vector2 mouseDirection = new Vector2(mouseSpeedX, mouseSpeedY).normalized;
            Vector2 force = mouseDirection * mouseSpeed * throwForceMulti;
            rigid.isKinematic = false;
            rigid.AddForce(force);

            GetComponent<Health>().canTakeDamage = true;

            meshRenderer.material = normalMaterial;
            TransitionToCooldown();
            return;
        }

        if(tickTimer < 0){
            player.gameObject.SendMessage("OnHit", new DamageInstance(damage, airDamage));
            tickTimer = timeBetweenTicks;
        }
        else 
            tickTimer -= Time.deltaTime;
    }

    private void TransitionToCooldown() {
        cdTimer = cdTime;
        state = ChargerState.cooldown;
    }

    private void Cooldown() {
        if (cdTimer <= 0){
            if(!rigid.isKinematic)
                rigid.isKinematic = true;

            TransitionToCharging();
        }
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

