using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    private static TransitionManager _instance;
    public static TransitionManager Instance { get => _instance; }
    public Transition transition;

    public static event System.Action OnTransitionComplete;
    public static event System.Action OnTransitionStart;

    string levelNameToTransitionTo;
    bool isTransitioning;

    // Start is called before the first frame update
    void Start()
    {
        if (_instance != null && _instance != this) {
            Destroy(this);
            return;
        }
        _instance = this;

        transition?.PlayInTransition();
    }

    public void TransitionCompleted() {
        OnTransitionComplete?.Invoke();
        isTransitioning = false;
        OnTransitionComplete = null;
    }

    public void TransitionToLevel(string levelName) {
        if (isTransitioning) return;

        OnTransitionStart?.Invoke();
        isTransitioning = true;
        transition?.PlayOutTransition();
        levelNameToTransitionTo = levelName;
        OnTransitionComplete += LoadLevel;
    }

    public void TransitionToEvent() {

    }

    public void LoadLevel() {
        LoadLevel(levelNameToTransitionTo);
    }

    public void LoadLevel(string levelName) {
        SceneManager.LoadScene(levelName);
    }
}
