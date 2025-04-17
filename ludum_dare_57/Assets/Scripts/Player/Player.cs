using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    private CharacterController controller;
    Camera cam;
    Transform camTrans;
    bool isPaused;

    public float deadzone = 0.75f;
    public float rotationSpeed = 10f;
    [Space]
    public float minX = 1f;
    public float maxX = 120f;
    [Space]
    public float distanceMultiplier = 2f;
    [Space]
    public float dodgeTimer = 1f;
    public bool isDodging;
    public float dgTimer;
    //public float refillSpeed = 0.25f;
    public AnimationCurve refillSpeedCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.25f));
    public float dodgeSpeed = 4f;
    public int dodgeCost = 1;
    [Space]
    public int dodgeDamage = 100;
    public int collisionDamage = 1;
    public int collisionAirDamage = 1;
    public float ramForce = 50f;

    Vector3 dodgeMovement;

    public AudioSource dodgeSound;
    [Space]
    public AudioClip metal;
    Health health;

    private HashSet<GameObject> collidedObjects = new HashSet<GameObject>();

    private const float clearColliderTimer = 2f;

    private const string OnRamStr = "OnRam";
    private const string EnemyTagStr = "Enemy";
    private const string BubbleTagStr = "Bubble";
    private const string MineTagStr = "Mine";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = Camera.main;
        camTrans = cam.transform;
        health = GetComponent<Health>();

        InvokeRepeating(nameof(clearCollided), clearColliderTimer, clearColliderTimer);
    }

    void clearCollided() {
        collidedObjects.Clear();
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        // Check if the object has already been processed
        if (collidedObjects.Contains(hit.gameObject)) {
            return; // Skip processing if it's already handled
        }

        // Log the name of the object hit
        Debug.Log("Collided with: " + hit.gameObject.name);

        AudioManager.instance.PlaySoundOnPlayer(metal);

        collidedObjects.Add(hit.gameObject);

        // Check if the object has a Health component and send damage
        if (isDodging && hit.gameObject.TryGetComponent<Health>(out Health hp)) {
            hp.gameObject.SendMessage(Health.OnHitString, new DamageInstance(dodgeDamage, 0));
            hp.gameObject.SendMessage(OnRamStr, (transform.position - hit.transform.position) * -ramForce);
            //dgTimer = 0;
        } else {
            if (hit.gameObject.CompareTag(EnemyTagStr) || hit.gameObject.CompareTag(BubbleTagStr)) return;
            if (hit.gameObject.CompareTag(MineTagStr)) hit.gameObject.SendMessage(Health.OnHitString, new DamageInstance(100, 100), SendMessageOptions.DontRequireReceiver);
            gameObject.SendMessage(Health.OnHitString, new DamageInstance(collisionDamage, collisionAirDamage), SendMessageOptions.DontRequireReceiver);  //We hit something, take damage
        }
    }

    public void OnDead() {
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
        Cursor.visible = true;
    }

    public void SetPaused(bool paused) {
        if (paused) {
            Cursor.lockState = CursorLockMode.None;
            isPaused = true;
            Cursor.visible = true;
        }
        else
        {
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update() {
        if (isPaused) { return; }

        CameraBasedVerticalAndHorizontalMovement();
        AltitudeControls();
        HandleRotation();
        HandleDodging();
    }

    private void CameraBasedVerticalAndHorizontalMovement() {
        Vector3 movement = GetMovementVector();
        movement = Quaternion.LookRotation(camTrans.forward) * movement;
        controller.Move(movement * moveSpeed * Time.deltaTime);
    }

    private void AltitudeControls() {
        float moveY = Input.GetAxis("Altitude") * moveSpeed;
        Vector3 verticalMovement = new Vector3(0, moveY, 0);
        controller.Move(verticalMovement * Time.deltaTime);
    }

    private void HandleRotation() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float yaw = mouseX * rotationSpeed * Time.deltaTime;
        float pitch = -mouseY * rotationSpeed * Time.deltaTime;

        // Rotate around local Y (yaw)
        transform.Rotate(Vector3.up, yaw, Space.Self);

        // Rotate around local X (pitch)
        transform.Rotate(Vector3.right, pitch, Space.Self);

        // Reset roll by zeroing out the Z axis
        Vector3 euler = transform.localEulerAngles;
        euler.z = 0f;
        transform.localEulerAngles = euler;
    }

    private void HandleDodging() {
        Vector3 movement = GetMovementVector();

        if (dgTimer >= dodgeTimer && Input.GetKeyDown(KeyCode.LeftShift) && movement.magnitude != 0) {
            isDodging = true;
            dodgeSound.Play();
            movement = Quaternion.LookRotation(camTrans.forward) * movement;
            Vector3 dodgeDirection = movement;
            if (dodgeMovement == Vector3.zero) dodgeMovement = dodgeDirection * moveSpeed * dodgeSpeed;
            GameManager.instance.DamageAir(2);
            health.canTakeDamage = false;
        }

        if (dgTimer > 0 && isDodging) {
            controller.Move(dodgeMovement * Time.deltaTime);
            dgTimer -= Time.deltaTime;
        }
        else {
            isDodging = false;
            dodgeMovement = Vector3.zero;
            health.canTakeDamage = true;
        }

        if (!isDodging) {
            if (dgTimer <= dodgeTimer)
                dgTimer += Time.deltaTime * refillSpeedCurve.Evaluate(GameManager.instance.difficulty);
        }
    }

    private Vector2 GetMouseVectorBasedOnScreen() {
        float normalizedX = (Input.mousePosition.x / Screen.width) * 2f - 1f;
        float normalizedY = (Input.mousePosition.y / Screen.height) * 2f - 1f;

        Vector2 normalizedPos = new Vector2(normalizedX, normalizedY);
        return normalizedPos;
    }

    public Vector2 GetPlayerMousePos() {
        Vector2 normalizedPos = GetMouseVectorBasedOnScreen();
        return normalizedPos;
    }

    Vector3 GetMovementVector() {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;
        Vector3 movement = new Vector3(moveX, 0, moveZ);
        if (movement.magnitude > 1) movement = movement.normalized;
        return movement;
    }
}
