using UnityEngine;

public class SniperFish : MonoBehaviour
{
    public float activationDistance = 15f;
    Player player;
    Health playerHealth;
    Transform attackPoint;

    LineRenderer lineRenderer;

    public float timeBeforeSnipeLands = 2f;
    [SerializeField] AnimationCurve timeDifficulty;
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
    [SerializeField] ParticleSystem bubbleFire;

    bool isSniping;

    const float whiteThresh = 0.75f;
    const float redThresh = 0.5f;

    public Gradient nonWhiteGradient;
    Material laserMaterial;
    public float boostAmount = 1f;
    Threat threat;

    public enum SniperState {
        idle,
        charging,
        cooldown
    }
    public SniperState state;

    public float cdTime = 2f;
    float cdTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackPoint = Player.Instance.attackPoint;

        lineRenderer = GetComponent<LineRenderer>();
        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();

        laserMaterial = lineRenderer.material;
        laserMaterial.EnableKeyword("_EMISSION");
        threat = GetComponent<Threat>();

    }

    public void OnRam(Vector3 dir) {
        timer = cooldown;
        isSniping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameManager.GameState.playing) return;

        var diffMulti = timeDifficulty.Evaluate(GameManager.instance.difficulty);

        switch (state) {
            case SniperState.idle:
                if (AIHelpers.CanThePlayerSeeUs(transform, player.transform, activationDistance, 0f, dotRequirement, layerMask)) 
                    TransitionToCharging();
                break;
            case SniperState.charging:
                HandleCharging(diffMulti);
                break;
            case SniperState.cooldown:
                Cooldown();
                break;
        }
    }

    private void SetLaserColor(float diffMulti) {
        var playerDodgeTime = player.dodgeTimer * diffMulti;
        if (timer <= playerDodgeTime * whiteThresh)
            lineRenderer.material = laser3;
        else if (timer <= timeBeforeSnipeLands * redThresh)
            lineRenderer.material = laser2;
        else
            lineRenderer.material = laser1;
    }

    private void TransitionToCharging() {
        state = SniperState.charging;
        start.Play();
        threat.BecomeThreat();
        lineRenderer.enabled = true;
        windup.Play();
        timer = timeBeforeSnipeLands;
    }

    private void HandleCharging(float diffMulti) {
        //Check for line of sight
        bool hasPlayersLineOfSight = !Physics.Linecast(transform.position, attackPoint.position, layerMask);
        if (!hasPlayersLineOfSight) {
            //Lost line of sight, exit sniping
            //timer = timeBeforeSnipeLands;
            //lineRenderer.enabled = false;
            //lineRenderer.SetPosition(0, MouthPoint.position);
            //lineRenderer.SetPosition(1, attackPoint.position); //TODO move to hitPoint
            //isSniping = false;
            TransitionToCooldown();
        }

        //Look at the target
        transform.LookAt(attackPoint.position);

        //Place line renderer 
        lineRenderer.SetPosition(0, MouthPoint.position);
        lineRenderer.SetPosition(1, attackPoint.position);

        //Calculate line renderer color
        SetLaserColor(diffMulti);

        //windup adjust pitch
        float pitchFactor = 1 - (timer / timeBeforeSnipeLands);
        windup.pitch = Mathf.Lerp(1.0f, maxPitch, pitchFactor);

        //Check timer, on zero Fire gun
        timer -= diffMulti * Time.deltaTime;
        if (timer < 0) {
            FireGunTransitonToCooldown(hasPlayersLineOfSight);
        }
    }

    private void FireGunTransitonToCooldown(bool hasPlayersLineOfSight) {
        lineRenderer.enabled = false;
        gunshot.Play();
        gunSmoke.Play();

        //Create a line of bubbles for a gunshot
        bubbleFire.transform.position = MouthPoint.position;
        bubbleFire.transform.LookAt(attackPoint.position);
        bubbleFire.Play();

        //hasPlayersLineOfSight = !Physics.Linecast(transform.position, attackPoint.position, layerMask);  //TODO is this really necessary cuz we are checking for LOS above
        if (hasPlayersLineOfSight && !player.isDodging) {
            playerHealth.SendMessage(Health.OnHitString, new DamageInstance(damage, airDamage));
        }

        TransitionToCooldown();
    }

    private void TransitionToCooldown() {
        state = SniperState.cooldown;
        timer = cooldown;
        cdTimer = cooldown;

        lineRenderer.enabled = false;
        var diffMulti = timeDifficulty.Evaluate(GameManager.instance.difficulty);
        timer -= diffMulti * Time.deltaTime;  //We are in cooldown
        start.Stop();
        windup.Stop();
        isSniping = false;
        threat.UnBecomeThreat();
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

//Can we snipe
//Are we in range
//Can the player see us?
//Can we see the player?
//Is there an avaliable snipe slot?
