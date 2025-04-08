using UnityEngine;

public class SniperFish : MonoBehaviour
{
    public float activationDistance = 15f;
    Player player;
    Health playerHealth;
    Transform attackPoint;

    LineRenderer lineRenderer;

    public float timeBeforeSnipeLands = 2f;
    public float cooldown = 10f;
    float timer;

    public float maxPitch = 1.5f;

    public int damage = 50;
    public int airDamage = 2;

    public float dotRequirement = -0.65f;
    public LayerMask layerMask = ~0;

    [SerializeField] AudioSource start;
    [SerializeField] AudioSource gunshot;
    [SerializeField] AudioSource windup;

    [SerializeField] Material laser1, laser2, laser3;

    [SerializeField] Transform MouthPoint;

    [SerializeField] ParticleSystem gunSmoke;

    bool isSniping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackPoint = GameObject.Find("AttackPoint").transform;

        lineRenderer = GetComponent<LineRenderer>();
        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();
    }

    public void OnRam(Vector3 dir) {
        timer = cooldown;
        isSniping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameManager.GameState.playing) return;

        if (timer > timeBeforeSnipeLands) {
            lineRenderer.enabled = false;
            timer -= Time.deltaTime;  //We are in cooldown
            start.Stop();
            windup.Stop();
            isSniping = false;
            gameObject.SendMessage(Threat.unBecomeThreat);
            return;
        }

        if (isSniping) {
            transform.LookAt(attackPoint.position);
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, MouthPoint.position);
            lineRenderer.SetPosition(1, attackPoint.position);

            if (timer <= timeBeforeSnipeLands * 0.3f)
                lineRenderer.material = laser3;
            else if (timer <= timeBeforeSnipeLands * 0.6f)
                lineRenderer.material = laser2;
            else
                lineRenderer.material = laser1;

            if (!windup.isPlaying) {
                windup.Play();
            }

            float pitchFactor = 1 - (timer / timeBeforeSnipeLands);
            windup.pitch = Mathf.Lerp(1.0f, maxPitch, pitchFactor);

            timer -= Time.deltaTime;
            if (timer < 0) {
                lineRenderer.enabled = false;
                if (!gunshot.isPlaying) gunshot.Play();
                gunSmoke.Play();

                bool hasPlayersLineOfSight = !Physics.Linecast(transform.position, attackPoint.position, layerMask);
                if(hasPlayersLineOfSight && !player.isDodging) {
                    playerHealth.SendMessage(Health.OnHitString, new DamageInstance(damage, airDamage));
                }

                timer = cooldown;
            }
        }
        else {
            lineRenderer.enabled = false;
            timer = timeBeforeSnipeLands;
        }

        if (!isSniping && AIHelpers.canThePlayerSeeUs(transform, attackPoint, activationDistance, 0f, dotRequirement, layerMask)) {
            StartSniping();
        }
    }

    private void StartSniping() {
        if(isSniping) { return; }

        isSniping = true;
        start.Play();
        gameObject.SendMessage(Threat.becomeThreatString);
    }

    public void OnHit(DamageInstance damageInstance) {
        StartSniping();
    }

    //Can we snipe
    //Are we in range
    //Can the player see us?
    //Can we see the player?
    //Is there an avaliable snipe slot?

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
