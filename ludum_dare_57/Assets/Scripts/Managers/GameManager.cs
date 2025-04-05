using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int gemCount;
    public float air = 10;

    public GameState currentGameState;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject deadCanvas;
    [SerializeField] GameObject inGameCanvas;
    [SerializeField] GameObject tutorialCanvas;
    [SerializeField] GameObject wonCanvas;

    Player player;
    public float surfacingHeight = 20f;

    public enum GameState {
        playing,
        dead,
        won,
        paused
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(currentGameState == GameState.playing && gemCount >= 5) {
        //    WinGame();  // you win by getting out
        //}

        if (currentGameState == GameState.playing)
            air -= Time.deltaTime;

        if(air < 0) {
            player.GetComponent<Health>().OnHit(1000);
        }

        if(currentGameState == GameState.playing && player.transform.position.y > surfacingHeight) {
            WinGame();
        }
    }

    private void WinGame() {
        currentGameState = GameState.won;
        if (pauseCanvas) pauseCanvas?.SetActive(false);
        if (deadCanvas) deadCanvas?.SetActive(false);
        if (inGameCanvas) inGameCanvas?.SetActive(false);
        if (tutorialCanvas) tutorialCanvas?.SetActive(false);
        if (wonCanvas) wonCanvas?.SetActive(true);
    }

    public void PlayerDeath() {
        if (pauseCanvas) pauseCanvas?.SetActive(false);
        if (deadCanvas) deadCanvas?.SetActive(true);
        if (inGameCanvas) inGameCanvas?.SetActive(false);
        if (tutorialCanvas) tutorialCanvas?.SetActive(false);
        if (wonCanvas) wonCanvas?.SetActive(false);
    }

    public void AddGem(int value) {
        gemCount += value;
    }
}
