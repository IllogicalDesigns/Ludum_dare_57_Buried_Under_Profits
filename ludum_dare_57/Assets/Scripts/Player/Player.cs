using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    private CharacterController controller;
    Camera cam;
    Transform camTrans;
    bool isPaused;

    public float deadzone = 0.75f;
    public float rotationSpeed = 100f;
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

    Vector3 dodgeMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Confined;
        cam = Camera.main;
        camTrans = cam.transform;
    }

    public void SetPaused(bool paused) {
        if (paused) {
            Cursor.lockState = CursorLockMode.None;
            isPaused = true;
        }
        else
        {
            isPaused = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    // Update is called once per frame
    void Update() {
        if (isPaused) { return; }

        CameraBasedVerticalAndHorizontalMovement();

        AltitudeControls();

        Vector2 normalizedPos = GetMouseVectorBasedOnScreen();

        if (normalizedPos.magnitude > deadzone) {
            HandleRotation(normalizedPos);
        }

        Vector3 movement = GetMovementVector();

        if (dgTimer >= dodgeTimer && Input.GetKeyDown(KeyCode.LeftShift)) {
            isDodging = true;
            movement = Quaternion.LookRotation(camTrans.forward) * movement;
            Vector3 dodgeDirection = movement;
            if(dodgeMovement == Vector3.zero) dodgeMovement = dodgeDirection * moveSpeed * dodgeSpeed;
            GameManager.instance.DamageAir(1);
        }

        if (dgTimer > 0 && isDodging) {
            controller.Move(dodgeMovement * Time.deltaTime);
            dgTimer -= Time.deltaTime;
        } else {
            isDodging = false;
            dodgeMovement = Vector3.zero;
        }

        if (!isDodging) {
            if(dgTimer <= dodgeTimer)
                dgTimer += Time.deltaTime * refillSpeed;
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

    private void HandleRotation(Vector2 normPos) {
        Vector3 currentRotation = transform.localEulerAngles;

        float distanceMulti = normPos.magnitude * distanceMultiplier;

        float currentXRotation = currentRotation.x;
        if (currentXRotation > 180) currentXRotation -= 360;
        float newXRotation = currentXRotation + normPos.y * -rotationSpeed * distanceMulti * Time.deltaTime;

        float currentYRotation = currentRotation.y;
        float newYRotation = currentYRotation + -normPos.x * -rotationSpeed * distanceMulti * normPos.magnitude * Time.deltaTime;

        transform.localEulerAngles = new Vector3(newXRotation, newYRotation, currentRotation.z);
    }

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
