using UnityEngine;

public class FullscreenManager : MonoBehaviour {
    const string isFullscreen = "isFullscreen";
    public bool defaultIsFullscreen = true;
    public bool isFullscreenValue;

    void Start() {
        isFullscreenValue = PlayerPrefs.GetInt(isFullscreen) == 1;

#if UNITY_WEBGL
        this.enabled = false; 
        return;
#endif

        //if (Screen.fullScreen != isFullscreenValue)
            ToggleFullscreen(isFullscreenValue);
    }

    public void OnFullscreenToggle(bool value) {
        Debug.Log($"Fullscreen toggled: {value}");
        ToggleFullscreen(value);
        PlayerPrefs.SetInt(isFullscreen, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    void ToggleFullscreen(bool value) {
        Screen.fullScreen = value;
    }
}