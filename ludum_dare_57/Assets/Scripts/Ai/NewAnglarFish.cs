using DG.Tweening;
using UnityEngine;

public class NewAnglarFish : MonoBehaviour {
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

    [SerializeField] AudioSource start;
    [SerializeField] AudioSource prechargeSFX;
    [SerializeField] AudioSource swim;
    [SerializeField] AudioSource stunned;

    public float dotRequirement = -0.85f;
    public LayerMask layerMask = ~0;

    float coolDown = 2f;
    float timer;

    public enum AnglarState {
        hiding,
        intro,
        precharge,
        charge,
        stunned
    }

    public ParticleSystem sandSystem;

    public AnglarState state;

    [Space]
    Vector3 introChargePosition;
    public float introChargeSpeed = 8f;
    public float introChagePercent = 0.8f;


    [Space]
    public float preChageTime = 1f;
    float preChargeTimer;

    [Space]
    Vector3 chargePosition;
    public float chargeSpeed = 8f;
    public float chagePercent = 2f;
    public float maxChargeDistance = 20f;
    public LayerMask chargeLayerMask;
    bool shouldStun;
    public Ease chargeEase = Ease.Linear;

    [Space]
    public float stunTime = 5f;
    float stunTimer;

    bool playerStruckThisCharge;

    Tween chargeTween;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        attackPoint = Player.Instance.attackPoint;
        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.currentGameState != GameManager.GameState.playing) return;

        if (timer > 0 && !isCharging) {
            timer -= Time.deltaTime;
        }

        var distance = Vector3.Distance(transform.position, attackPoint.position);

        switch(state) {
            case AnglarState.hiding:
                if (distance < activationDistance)
                    TransitionToIntro();
                break;
            case AnglarState.intro:
                HandleIntro();

                break;
            case AnglarState.precharge:
                HandlePreCharge();

                break;
            case AnglarState.charge:
                HandleCharge(distance);

                break;
            case AnglarState.stunned:
                if (stunTimer <= 0)
                    TransitionToPreCharge();
                else
                    stunTimer -= Time.deltaTime;
                break;
            default: 
                //How did you get here?
                break;
        }
    }



    void TransitionToIntro() {
        //calculate a jump towards the player and chomp
        state = AnglarState.intro;
        introChargePosition = Vector3.Lerp(transform.position, attackPoint.position, introChagePercent);
        sandSystem.Play();
        if (start != null) start.Play();
    }

    private void HandleIntro() {
        //Follow the intro calculation
        if (chargeTween == null)
            chargeTween = transform.DOMove(introChargePosition, introChargeSpeed).SetSpeedBased(true).OnComplete(() => { TransitionToPreCharge(); chargeTween = null; });
    }

    void TransitionToPreCharge() {
        state = AnglarState.precharge;
        preChargeTimer = preChageTime;
        if(prechargeSFX != null) prechargeSFX.Play();
        if (swim != null) swim.Stop();
    }

    private void HandlePreCharge() {
        //hold, turn towards player, precharge animation
        transform.LookAt(attackPoint.position);

        if (preChargeTimer <= 0)
            TransitionToCharge();
        else
            preChargeTimer -= Time.deltaTime;
    }

    void TransitionToCharge() {
        state = AnglarState.charge;

        playerStruckThisCharge = false;

        Vector3 origin = transform.position;
        Vector3 target = attackPoint.position;
        Vector3 direction = (target - origin).normalized;

        if (swim != null) swim.Play();

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, maxChargeDistance, chargeLayerMask)) {
            // The ray hit something between origin and target
            Debug.Log("Hit: " + hit.collider.name);
            chargePosition = hit.point;
            shouldStun = true;
        } else {
            chargePosition = transform.position + direction * maxChargeDistance;
            Debug.Log("No hit, charging to max dist ");
            shouldStun = false;
        }
    }

    private void HandleCharge(float distance) {
        //Charge through the player, straight line, if it hits a wall, see stunned
        if (chargeTween == null)
            chargeTween = transform.DOMove(chargePosition, chargeSpeed).SetSpeedBased(true).OnComplete(() => { TransitionToStun(); chargeTween = null; }).SetEase(chargeEase);

        if (distance < impactDistance && !playerStruckThisCharge) {
            Debug.Log("Hitting player");
            playerStruckThisCharge = true;
            playerHealth.SendMessage(Health.OnHitString, new DamageInstance(damage, airDamage));
            shouldStun = false;
            chargeTween.Kill();
            TransitionToPreCharge();
        }
    }

    void TransitionToStun() {
        stunTimer = stunTime;

        if (stunned != null) stunned.Play();
        if (swim != null) swim.Stop();

        if (shouldStun)
            state = AnglarState.stunned;
        else
            TransitionToPreCharge();
    }

    public void OnHit(DamageInstance damageInstance) {
        if(state == AnglarState.hiding) {
            TransitionToIntro();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}


