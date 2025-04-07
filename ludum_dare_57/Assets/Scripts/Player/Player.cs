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

    public float dodgeTimer = 1f;
    public bool isDodging;
    public float dgTimer;
    public float refillSpeed = 0.25f;
    public float dodgeSpeed = 4f;
    public int dodgeCost = 1;

    public float sloMoSpeed = 0.5f;
    public float normalSpeed = 1f;

    public int dodgeDamage = 100;
    public int collisionDamage = 1;
    public int collisionAirDamage = 1;
    public float ramForce = 50f;

    Vector3 dodgeMovement;

    public AudioSource dodgeSound;
    public AudioClip metal;
    Health health;

    private HashSet<GameObject> collidedObjects = new HashSet<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = Camera.main;
        camTrans = cam.transform;
        health = GetComponent<Health>();

        InvokeRepeating(nameof(clearCollided), 2f, 2f);
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
            hp.gameObject.SendMessage("OnHit", new DamageInstance(dodgeDamage, 0));
            hp.gameObject.SendMessage("OnRam", (transform.position - hit.transform.position) * -ramForce);
            //dgTimer = 0;
        } else {
            if (hit.gameObject.CompareTag("Enemy")) return;
            if(hit.gameObject.CompareTag("Mine")) hit.gameObject.SendMessage("OnHit", new DamageInstance(100, 100), SendMessageOptions.DontRequireReceiver);
            gameObject.SendMessage("OnHit", new DamageInstance(collisionDamage, collisionAirDamage), SendMessageOptions.DontRequireReceiver);  //We hit something, take damage
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

        Time.timeScale = Input.GetKey(KeyCode.Mouse1) ? sloMoSpeed : normalSpeed;

        CameraBasedVerticalAndHorizontalMovement();

        AltitudeControls();

        //The below code is panned by critics, so its gone
        //Vector2 normalizedPos = GetMouseVectorBasedOnScreen();
        //if (normalizedPos.magnitude > deadzone) {
        //    HandleRotation(normalizedPos);
        //}

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        HandleRotation(mouseX, mouseY);

        Vector3 movement = GetMovementVector();

        if (dgTimer >= dodgeTimer && Input.GetKeyDown(KeyCode.LeftShift) && movement.magnitude != 0) {
            isDodging = true;
            dodgeSound.Play();
            movement = Quaternion.LookRotation(camTrans.forward) * movement;
            Vector3 dodgeDirection = movement;
            if(dodgeMovement == Vector3.zero) dodgeMovement = dodgeDirection * moveSpeed * dodgeSpeed;
            GameManager.instance.DamageAir(2);
            health.canTakeDamage = false;
        }

        if (dgTimer > 0 && isDodging) {
            controller.Move(dodgeMovement * Time.deltaTime);
            dgTimer -= Time.deltaTime;
        } else {
            isDodging = false;
            dodgeMovement = Vector3.zero;
            health.canTakeDamage = true;
        }

        if (!isDodging) {
            if(dgTimer <= dodgeTimer)
                dgTimer += Time.deltaTime * refillSpeed;
        }
    }

    private void HandleRotation(float mouseX, float mouseY) {
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

    //The below code is panned by critics, so its gone
    //private void HandleRotation(Vector2 normPos) {
    //    Vector3 currentRotation = transform.localEulerAngles;

    //    float distanceMulti = normPos.magnitude * distanceMultiplier;

    //    float currentXRotation = currentRotation.x;
    //    if (currentXRotation > 180) currentXRotation -= 360;
    //    float newXRotation = currentXRotation + normPos.y * -rotationSpeed * distanceMulti * Time.deltaTime;

    //    float currentYRotation = currentRotation.y;
    //    float newYRotation = currentYRotation + -normPos.x * -rotationSpeed * distanceMulti * normPos.magnitude * Time.deltaTime;

    //    transform.localEulerAngles = new Vector3(newXRotation, newYRotation, currentRotation.z);
    //}

    private void AltitudeControls() {
        float moveY = Input.GetAxis("Altitude") * moveSpeed;
        Vector3 verticalMovement = new Vector3(0, moveY, 0);
        controller.Move(verticalMovement * Time.deltaTime);
    }

    Vector3 GetMovementVector() {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;
        Vector3 movement = new Vector3(moveX, 0, moveZ);
        if (movement.magnitude > 1) movement = movement.normalized;
        return movement;
    }

    private void CameraBasedVerticalAndHorizontalMovement() {
        Vector3 movement = GetMovementVector();
        movement = Quaternion.LookRotation(camTrans.forward) * movement;
        controller.Move(movement * moveSpeed * Time.deltaTime);
    }
}
