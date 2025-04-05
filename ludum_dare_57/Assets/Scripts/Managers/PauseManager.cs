using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseCanvas;
    bool isPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseCanvas.SetActive(isPaused);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }   
    }

    public void TogglePause() {
        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);
    }
}
