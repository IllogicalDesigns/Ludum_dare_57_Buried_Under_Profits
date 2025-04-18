using UnityEngine;

public class ChargeFish : MonoBehaviour
{
    public float activationDistance = 15f;
    public float minActivationDistance = 5f;
    Player player;
    Transform attackPoint;
    Health playerHealth;

    public float speed = 5f;

    bool isCharging;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackPoint = Player.Instance.attackPoint;
        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameManager.GameState.playing) return;

        var distance = Vector3.Distance(transform.position, attackPoint.position);

        if (!isCharging && AIHelpers.CanThePlayerSeeUs(transform, player.transform, activationDistance, minActivationDistance, dotRequirement, layerMask)) {
            StartCharging();
        }

        if (isCharging) {
            transform.LookAt(attackPoint.position);

            Vector3 targetPos = attackPoint.position;
            
            if (distance < (activationDistance * 0.75) && distance > minActivationDistance) {       //For some distance offset our main vector by this rand vector
                targetPos = attackPoint.position + new Vector3(jitterX, jitterY, jitterZ);          //This prevents straight line syndrome
            }
            
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (distance < distanceToHit) {
                //impact
                playerHealth.SendMessage(Health.OnHitString, new DamageInstance(damage, airDamage));
                isCharging = false;
                gameObject.SendMessage(Threat.unBecomeThreat);
                swim.Stop();
            }
        }
    }

    private void StartCharging() {
        if (isCharging) return;

        jitterX = Random.Range(-jitterRange, jitterRange);
        jitterY = Random.Range(-jitterRange, jitterRange);
        jitterZ = Random.Range(-jitterRange, jitterRange);

        isCharging = true;
        gameObject.SendMessage(Threat.becomeThreatString);
        start.Play();
        swim.Play();
    }

    public void OnHit(DamageInstance damageInstance) {
        StartCharging();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
