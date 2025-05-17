using UnityEngine;

public class SniperFish : MonoBehaviour
{
    public LayerMask layerMask = ~0;

    [Header("Difficulty")]
    [SerializeField] float activationDistance = 15f;
    [SerializeField] float dotRequirement = -0.65f;
    [SerializeField] float timeBeforeSnipeLands = 2f;
    [SerializeField] AnimationCurve timeDifficulty;
    [SerializeField] float cooldown = 10f;
    [SerializeField] int damage = 50;
    [SerializeField] int airDamage = 2;

    [SerializeField] Transform MouthPoint;

    [Header("Particles")]
    [SerializeField] ParticleSystem gunSmoke;
    [SerializeField] ParticleSystem bubbleFire;

    [Header("Audio")]
    [SerializeField] AudioSource startSFX;
    [SerializeField] AudioSource gunshotSFX;
    [SerializeField] AudioSource windupSFX;
    public float maxPitch = 1.5f;

    bool isSniping;
    Threat threat;

    [Header("SniperLaser")]
    public Material preWhiteMaterial;
    public Material whiteMaterial;

    public float whiteThreshold = 1.5f;
    public float maxTime = 2f;

    [Range(0.0f, 3f)]
    public float timerBeforeShot;

    public bool autoAdvance = false;

    public Gradient preWhiteGradient;
    public float emissionIntensity = 5.0f;

    [Header("OLD_SniperLaser")]
    public Gradient nonWhiteGradient;
    LineRenderer lineRenderer;
    const float whiteThresh = 0.75f;
    const float redThresh = 0.5f;
    Material laserMaterial;
    public float boostAmount = 1f;
    [SerializeField] Material laser1, laser2, laser3;

    Player player;
    Health playerHealth;
    Transform attackPoint;
    float timer;

    public enum SniperState {
        idle,
        charging,
        cooldown
    }
    public SniperState state;

    public float cdTime = 2f;
    float cdTimer;

    float laserColorTimer;

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
        if (timer <= playerDodgeTime * whiteThresh) {
            lineRenderer.material = laser3;
        }
        else {
            lineRenderer.material = laser1;

            float threshold = playerDodgeTime * whiteThresh;
            laserColorTimer += Time.deltaTime;

            lineRenderer.material.SetColor("_EmissionColor", nonWhiteGradient.Evaluate(laserColorTimer/(timeBeforeSnipeLands-threshold)));
        }
    }

    private void SetLaserColorNEW(float diffMulti) {
        var playerDodgeTime = player.dodgeTimer * diffMulti;

        timerBeforeShot = timer;

        //lineRenderer.material = timerBeforeShot < player.dodgeTimer ? whiteMaterial : preWhiteMaterial;
        if(timer < (playerDodgeTime)) {
            lineRenderer.material.EnableKeyword("_EMISSION");
            lineRenderer.material.SetColor("_EmissionColor", Color.white * 5f);
        }
        else
        {
            var lerped = Mathf.Lerp(0, timeBeforeSnipeLands - whiteThreshold, timerBeforeShot);

            Color finalColor = preWhiteGradient.Evaluate(lerped) * emissionIntensity;
            preWhiteMaterial.color = finalColor;
            lineRenderer.material.EnableKeyword("_EMISSION");
            lineRenderer.material.SetColor("_EmissionColor", finalColor);
        }
    }

    private void TransitionToCharging() {
        laserColorTimer = 0;
        state = SniperState.charging;
        startSFX.Play();
        threat.BecomeThreat();
        lineRenderer.enabled = true;
        windupSFX.Play();
        timer = timeBeforeSnipeLands;
    }

    private void HandleCharging(float diffMulti) {
        //Check for line of sight
        bool hasPlayersLineOfSight = !Physics.Linecast(transform.position, attackPoint.position, layerMask);
        if (!hasPlayersLineOfSight) {
            //Lost line of sight, exit sniping
            //timer = timeBeforeSnipeLands;
            lineRenderer.enabled = false;
            //lineRenderer.SetPosition(0, MouthPoint.position);
            //lineRenderer.SetPosition(1, attackPoint.position); //TODO move to hitPoint
            //isSniping = false;
            //TransitionToCooldown();
            //return;
        } else
            lineRenderer.enabled = true;

        //Look at the target
        transform.LookAt(attackPoint.position);

        //Place line renderer 
        lineRenderer.SetPosition(0, MouthPoint.position);
        lineRenderer.SetPosition(1, attackPoint.position);

        //Calculate line renderer color
        SetLaserColorNEW(diffMulti);

        //windup adjust pitch
        float pitchFactor = 1 - (timer / timeBeforeSnipeLands);
        windupSFX.pitch = Mathf.Lerp(1.0f, maxPitch, pitchFactor);

        //Check timer, on zero Fire gun
        timer -= diffMulti * Time.deltaTime;
        if (timer < 0) {
            FireGunTransitonToCooldown(hasPlayersLineOfSight);
        }
    }

    private void FireGunTransitonToCooldown(bool hasPlayersLineOfSight) {
        lineRenderer.enabled = false;
        gunshotSFX.Play();
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
        startSFX.Stop();
        windupSFX.Stop();
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
        if(state == SniperState.idle)
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
