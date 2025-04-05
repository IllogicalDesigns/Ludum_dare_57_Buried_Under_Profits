using UnityEngine;

public class VolumeManager : MonoBehaviour {
    const string volume = "volume";
    public float defaultVolume = 0.5f;
    public float volumeValue;

    void Start() {
        volumeValue = PlayerPrefs.GetFloat(volume, defaultVolume);
        SetVoume(volumeValue);
    }

    public void OnVolumeChanged(float value) {
        Debug.Log($"Volume changed: {value}");
        value = Mathf.Clamp(value, 0f, 1f);
        SetVoume(value);
        PlayerPrefs.SetFloat(volume, value);
        PlayerPrefs.Save();
    }

    void SetVoume(float value) {
        AudioListener.volume = Mathf.Clamp(value, 0f, 1f);
    }
}
