using UnityEngine;


public class TriggerBubbleAmmo : TriggerBase {
    public AudioClip popSFX;
    public AnimationCurve ammoProvidedCurve;
    public Transform ammoBox;
    public MoveToPoint toPoint;
    Transform player;
    public AnimationCurve pickupDistance = new AnimationCurve(new Keyframe(0, 15), new Keyframe(2, 3));

    Rigidbody rb;
    public float force = 5f;
    //const float gravity = 1f;

    protected override void Awake() {
        base.Awake();
        player = Player.Instance.transform;
    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Random.onUnitSphere * force, ForceMode.Impulse);
    }


    protected override void OnTriggerEnter(Collider other) {
        if (!IsCorrectTag(other)) {
            return;
        }

        base.OnTriggerEnter(other);
        PopBubble();
    }

    private void PopBubble() {
        ammoBox.SetParent(null);
        toPoint.enabled = true;

        var gun = FindAnyObjectByType<PlayerGun>();
        // gun?.addAmmo());
        gun.gameObject.SendMessage("addAmmo", Mathf.RoundToInt(ammoProvidedCurve.Evaluate(gun.ammo)));
        FindAnyObjectByType<AmmoCounter>()?.ProvidedAmmo();
        AudioManager.instance.PlaySoundOnPoint(popSFX, transform.position);
        Destroy(gameObject);
    }

    public void OnHit(DamageInstance damageInstance) {
        PopBubble();
    }

    private void Update() {
        if (Vector3.Distance(transform.position, player.position) < pickupDistance.Evaluate(GameManager.instance.difficulty)) {
            PopBubble();
        }

        //rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration); // * Time.deltaTime
    }
}

