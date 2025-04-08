using UnityEngine;

public class DifficultyManager : MonoBehaviour {
    const string difficulty = "difficulty";
    public float defaultDifficulty = 1f;
    public float minDifficulty = 0.1f;
    public float maxDifficulty = 2f;
    public float difficultyValue;

    void Start() {
        difficultyValue = PlayerPrefs.GetFloat(difficulty, defaultDifficulty);
        SetDifficulty(difficultyValue);
    }

    public void OnDifficultyChanged(float value) {
        Debug.Log($"difficulty changed: {value}");
        value = Mathf.Clamp(value, 0.1f, 2f);
        SetDifficulty(value);
        PlayerPrefs.SetFloat(difficulty, value);
        PlayerPrefs.Save();
    }

    void SetDifficulty(float value) {
        if (GameManager.instance != null) {
            GameManager.instance.difficulty = value;
        }
    }
}

