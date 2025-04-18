using UnityEngine;

public class SloMotion : MonoBehaviour
{
    bool isPaused;

    [SerializeField] float sloMoSpeed = 0.5f;
    [SerializeField] float normalSpeed = 1f;
    //public float maxSlowTime = 5f;
    public AnimationCurve maxSlowTime = new AnimationCurve(new Keyframe(0f, 5f), new Keyframe(1f, 2f), new Keyframe(2f, 0.5f));
    public AnimationCurve refillRate = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(2f, 0.25f));
    public float slowTimer;

    public bool isSlowMotion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetPaused(bool paused) {
        if (paused) {
            Cursor.lockState = CursorLockMode.None;
            isPaused = true;
            Cursor.visible = true;
        }
        else {
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused) { return; }
        HandleSlowMotion();
    }

    private void HandleSlowMotion() {
        if (!isSlowMotion && slowTimer > 0) {
            Time.timeScale = normalSpeed;
            slowTimer -= Time.deltaTime * refillRate.Evaluate(GameManager.instance.difficulty);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && slowTimer <= maxSlowTime.Evaluate(GameManager.instance.difficulty)) {
            slowTimer = 0;
            isSlowMotion = true;
        }
        else if (Input.GetKey(KeyCode.Mouse1) && slowTimer < maxSlowTime.Evaluate(GameManager.instance.difficulty)) {
            slowTimer += Time.unscaledDeltaTime;
            Time.timeScale = sloMoSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1) || slowTimer >= maxSlowTime.Evaluate(GameManager.instance.difficulty)) {
            Time.timeScale = normalSpeed;
            GameManager.instance.DamageAir(Mathf.RoundToInt(slowTimer));
            isSlowMotion = false;
        } else if(slowTimer >= maxSlowTime.Evaluate(GameManager.instance.difficulty)) {
            Time.timeScale = normalSpeed;
            GameManager.instance.DamageAir(Mathf.RoundToInt(slowTimer));
            isSlowMotion = false;
        }
    }
}
