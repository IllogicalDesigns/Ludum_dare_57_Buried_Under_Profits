using UnityEngine;

public class SensitivityManager : MonoBehaviour {
    const string sens = "sens";
    public float maxSens = 300f;
    public float minSens = 1f;
    public float defaultSens = 150f;
    public float sensValue;

    void Start() {
        sensValue = PlayerPrefs.GetFloat(sens, defaultSens);
        SetSens(sensValue);
    }

    public void OnSensChanged(float value) {
        Debug.Log($"Sensitivity changed: {value}");
        value = Mathf.Clamp(value, minSens, maxSens);
        SetSens(value);
        PlayerPrefs.SetFloat(sens, value);
        PlayerPrefs.Save();
    }

    void SetSens(float value) {
        var player = FindAnyObjectByType<Player>();

        if(player != null) {
            player.rotationSpeed = value;
        }
    }
}