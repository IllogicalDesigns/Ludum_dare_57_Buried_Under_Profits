using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int gemCount;
    public float air = 10;
    public AnimationCurve airDecayCurve;

    public GameState currentGameState = GameState.waiting;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject deadCanvas;
    [SerializeField] GameObject inGameCanvas;
    [SerializeField] GameObject tutorialCanvas;
    [SerializeField] GameObject wonCanvas;

    [SerializeField] GameObject godActivated;
    [SerializeField] AudioClip bubbles;
    [SerializeField] AudioClip ammo;
    bool godMode;

    Player player;
    public float surfacingHeight = 20f;

    Health playerHealth;

    public float difficulty = 1f;

    bool goneUnderwater;

    public enum GameState {
        waiting,
        playing,
        dead,
        won,
        paused
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(currentGameState == GameState.playing && gemCount >= 5) {
        //    WinGame();  // you win by getting out
        //}

        if (Input.GetKeyDown(KeyCode.Keypad8)) {
            var gun = FindFirstObjectByType<PlayerGun>();
            gun.gameObject.SendMessage("addAmmo", 10);
        }

        if (currentGameState == GameState.playing)
            air -= airDecayCurve.Evaluate(air) * Time.deltaTime;

        if (godMode) air = 777;

        if (air < 0 && currentGameState == GameState.playing) {
            player.GetComponent<PlayerDeath>().OnDead();
        }

        if (currentGameState == GameState.waiting && player.transform.position.y < surfacingHeight)
        {
            currentGameState = GameState.playing;
            goneUnderwater = true;
        }

        if (goneUnderwater && currentGameState == GameState.playing && player.transform.position.y > surfacingHeight) {
            WinGame();
        }

        if(Input.GetKeyDown(KeyCode.Keypad0)) {
            if(godActivated) godActivated.SetActive(true);
            godMode = true;
            Debug.Log("Activated god mode");
            var playerHealthy = player.GetComponent<Health>();
            playerHealthy.hp = 10000;
            playerHealthy.maxHp = 10000;
        }
    }

    private void WinGame() {
        currentGameState = GameState.won;
        if (pauseCanvas) pauseCanvas?.SetActive(false);
        if (deadCanvas) deadCanvas?.SetActive(false);
        if (inGameCanvas) inGameCanvas?.SetActive(false);
        if (tutorialCanvas) tutorialCanvas?.SetActive(false);
        if (wonCanvas) wonCanvas?.SetActive(true);

        player.SetPaused(true);

        air = 60;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayerDeath() {
        currentGameState = GameState.dead;
        if (pauseCanvas) pauseCanvas?.SetActive(false);
        if (deadCanvas) deadCanvas?.SetActive(true);
        if (inGameCanvas) inGameCanvas?.SetActive(false);
        if (tutorialCanvas) tutorialCanvas?.SetActive(false);
        if (wonCanvas) wonCanvas?.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //TODO pause everything
    }

    public void AddGem(int value) {
        gemCount += value;
        FindAnyObjectByType<GemCounter>().BounceCounter();
    }

    public void DamageAir(int value) {
        if (!playerHealth.canTakeDamage) return;

        air -= value;
        FindAnyObjectByType<AirCounter>().DamagedAir(value);
    }

    public void ProvideAir(int value) {
        air += value;
        FindAnyObjectByType<AirCounter>().ProvidedAir(value);
        AudioManager.instance.PlaySoundOnPlayer(bubbles);
    }
}
