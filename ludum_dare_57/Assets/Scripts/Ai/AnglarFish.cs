using UnityEngine;

public class AnglarFish : MonoBehaviour {
    public float activationDistance = 6f;
    public float minActivationDistance = 5f;
    Player player;
    Transform attackPoint;
    Health playerHealth;

    public float speed = 5f;

    bool isCharging;

    public int damage = 50;
    public int airDamage = 10;

    public float impactDistance = 3f;

    public float jitterRange = 1f;
    public float jitterUpdate = 2f;
    float jitterX = 0f;
    float jitterY = 0f;
    float jitterZ = 0f;

    [SerializeField] AudioSource start;
    [SerializeField] AudioSource swim;

    public float dotRequirement = -0.85f;
    public LayerMask layerMask = ~0;

    float coolDown = 2f;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        attackPoint = GameObject.Find("AttackPoint").transform;
        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();

        InvokeRepeating(nameof(UpdateJitter), jitterUpdate, jitterUpdate);
    }

    void UpdateJitter() {
        jitterX = Random.Range(-jitterRange, jitterRange);
        jitterY = Random.Range(-jitterRange, jitterRange);
        jitterZ = Random.Range(-jitterRange, jitterRange);
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.currentGameState != GameManager.GameState.playing) return;

        if(timer > 0 && !isCharging) {
            timer -= Time.deltaTime;
        }

        var distance = Vector3.Distance(transform.position, attackPoint.position);
        if (!isCharging && timer <= 0 && AIHelpers.CanThePlayerSeeUs(transform, player.transform, activationDistance, minActivationDistance, dotRequirement, layerMask)) {
            StartCharge();
        }

        if (isCharging) {
            transform.LookAt(attackPoint.position);

            Vector3 targetPos = attackPoint.position;

            if (distance < (activationDistance * 0.75) && distance > minActivationDistance) { //For some distance offset out main vector by this rand vector
                targetPos = attackPoint.position + new Vector3(jitterX, jitterY, jitterZ);          //This prevents straight line syndrome
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (distance < impactDistance) {
                //impact
                Debug.Log("Hitting player");
                playerHealth.SendMessage(Health.OnHitString, new DamageInstance(damage, airDamage));
                isCharging = false;
                gameObject.SendMessage(Threat.unBecomeThreat);
                timer = coolDown;
                swim.Stop();
            }
        }
    }

    private void StartCharge() {
        if(isCharging) return;

        isCharging = true;
        gameObject.SendMessage(Threat.becomeThreatString);
        start.Play();
        swim.Play();
        timer = coolDown;
    }

    public void OnHit(DamageInstance damageInstance) {
        StartCharge();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}

