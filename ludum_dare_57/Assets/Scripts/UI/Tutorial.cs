using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject altitudePanel;
    [SerializeField] GameObject wASDPanel;
    [SerializeField] GameObject deadzonePanel;
    [SerializeField] GameObject shootPanel;
    [SerializeField] GameObject dodgePanel;
    [SerializeField] GameObject missionPanel;

    Player player;
    PlayerGun playerGun;
    [SerializeField] GameObject spawner;
    
    public float deadzone = 0.75f;

    public enum tutorialState {
        altitude,
        wasd,
        deadzone,
        shoot,
        dodge,
        mission,
        None
    }

    public tutorialState currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        altitudePanel.SetActive(true);
        wASDPanel.SetActive(false);
        deadzonePanel.SetActive(false);
        shootPanel.SetActive(false);
        dodgePanel.SetActive(false);

        player = FindAnyObjectByType<Player>();
        player.enabled = false;

        playerGun = FindAnyObjectByType<PlayerGun>();
        playerGun.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        altitudePanel.SetActive(false);
        wASDPanel.SetActive(false);
        deadzonePanel.SetActive(false);
        shootPanel.SetActive(false);
        dodgePanel.SetActive(false);
        missionPanel.SetActive(false);

        if (Input.GetKeyUp(KeyCode.T)) {
            Destroy(gameObject);
        }

        switch (currentState) {
            case tutorialState.altitude:
                altitudePanel.SetActive(true);
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Space))
                    currentState = tutorialState.wasd;
                break;
            case tutorialState.wasd:
                wASDPanel.SetActive(true);
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                    currentState = tutorialState.shoot;
                break;
            case tutorialState.deadzone:
                //deadzonePanel.SetActive(true);
                //if (player.GetPlayerMousePos().magnitude > deadzone)
                //    currentState = tutorialState.shoot;
                break;
            case tutorialState.shoot:
                shootPanel.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    currentState = tutorialState.dodge;
                break;
            case tutorialState.dodge:
                dodgePanel.SetActive(true);
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftShift))
                    currentState = tutorialState.mission;
                break;
            case tutorialState.mission:
                missionPanel.SetActive(true);
                if (Input.GetMouseButtonDown(0)) {
                    currentState = tutorialState.None;

                    spawner.SetActive(true);
                    player.enabled = true;
                    playerGun.enabled = true;

                    Destroy(gameObject);
                }
                break;
            default:
                break;
        }
    }
}
