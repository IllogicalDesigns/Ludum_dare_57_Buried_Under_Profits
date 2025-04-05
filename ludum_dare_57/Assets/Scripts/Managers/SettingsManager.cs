//using Unity.Cinemachine;
using UnityEngine;

public class SettingsManager : MonoBehaviour {
    const string volume = "volume";
    const string fov = "fov";
    const string sens = "sens";

    private static SettingsManager _instance;
    public static SettingsManager Instance { get => _instance; }

    public float defaultVolume = 0.5f;
    public float volumeValue;

    [Header("Field of view")]
    public float maxFov = 100f;
    public float minFov = 40f;
    public float defaultFov = 75f;
    public float fovValue;
    //[SerializeField] CinemachineCamera virtualCamera;

    [Header("Sensitivity")]
    public float maxSens = 300f;
    public float minSens = 1f;
    public float defaultSens = 150f;
    public float sensValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this);
            return;
        }
        _instance = this;

        volumeValue = PlayerPrefs.GetFloat(volume, defaultVolume);
        SetVolume(volumeValue);
        fovValue = PlayerPrefs.GetFloat(fov, defaultFov);
        SetFov(fovValue);
        sensValue = PlayerPrefs.GetFloat(sens, defaultSens);
        SetSens(sensValue);
    }

    public void OnVolumeChanged(float value) {
        Debug.Log($"Volume changed: {value}");
        value = Mathf.Clamp(value, 0f, 1f);
        SetVolume(value);
        PlayerPrefs.SetFloat(volume, value);
        PlayerPrefs.Save();
    }

    void SetVolume(float value) {
        value = Mathf.Clamp(value, 0f, 1f);
        AudioListener.volume = value;
    }

    public void OnFovChanged(float value) {
        Debug.Log($"FOV changed: {value}");
        SetFov(value);
        PlayerPrefs.SetFloat(fov, Mathf.Clamp(value, minFov, maxFov));
        PlayerPrefs.Save();
    }

    void SetFov(float value) {
        value = Mathf.Clamp(value, minFov, maxFov);
        Camera.main.fieldOfView = value;
        //virtualCamera.Lens.FieldOfView = value;
    }

    public void OnSensChanged(float value) {
        Debug.Log($"Sensitivity changed: {value}");
        value = Mathf.Clamp(value, minSens, maxSens);
        SetSens(value);
        PlayerPrefs.SetFloat(sens, value);
        PlayerPrefs.Save();
    }

    void SetSens(float value) {
        value = Mathf.Clamp(value, minSens, maxSens);

        //TODO set the rotation speed here for the player
    }
}
