using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] Player player;
    [SerializeField] PlayerGun playerGun;
    [SerializeField] SloMotion slowMotion;

    [SerializeField] GameObject tutorialCanvas;
    [SerializeField] GameObject gemCanvas;

    GameManager gameManager;
    GameManager.GameState lastGameState;

    bool isPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseCanvas.SetActive(isPaused);
        player = FindAnyObjectByType<Player>();
        playerGun = FindAnyObjectByType<PlayerGun>();
        slowMotion = FindAnyObjectByType<SloMotion>();
        gameManager = FindAnyObjectByType<GameManager>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }   
    }

    public void TogglePause() {


        if (gameManager.currentGameState != GameManager.GameState.paused) {
            lastGameState = gameManager.currentGameState;
            gameManager.currentGameState = GameManager.GameState.paused;
        }
        else
            gameManager.currentGameState = lastGameState;

        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);

        player.SetPaused(isPaused);
        playerGun.SetPaused(isPaused);
        slowMotion.SetPaused(isPaused);

        var bullets = FindObjectsOfType<Bullet>();
        foreach (Bullet bullet in bullets) {
            bullet.SetPaused(isPaused);
        }

        if (tutorialCanvas != null) 
            tutorialCanvas.SetActive(!isPaused);

        if (gemCanvas != null)
            gemCanvas.SetActive(!isPaused);
    }
}
