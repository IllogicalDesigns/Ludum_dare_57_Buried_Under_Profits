using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour {
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider fovSlider;
    [SerializeField] Slider sensSlider;
    [SerializeField] Toggle fullToggle;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [Space]
    [SerializeField] VolumeManager volumeManager;
    [SerializeField] FOVManager fovManager;
    [SerializeField] SensitivityManager sensManager;
    [SerializeField] FullscreenManager fullManager;
    [SerializeField] ResolutionManager resolutionManager;


    private void Awake() {
        volumeManager = FindAnyObjectByType<VolumeManager>();
        fovManager = FindAnyObjectByType<FOVManager>();
        sensManager = FindAnyObjectByType<SensitivityManager>();
        fullManager = FindAnyObjectByType<FullscreenManager>();
        resolutionManager = FindAnyObjectByType<ResolutionManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChanged(); });
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = volumeManager.volumeValue;

        fovSlider.onValueChanged.AddListener(delegate { OnFovChanged(); });
        fovSlider.minValue = fovManager.minFov;
        fovSlider.maxValue = fovManager.maxFov;
        fovSlider.value = fovManager.fovValue;

        sensSlider.onValueChanged.AddListener(delegate { OnSensChanged(); });
        sensSlider.minValue = sensManager.minSens;
        sensSlider.maxValue = sensManager.maxSens;
        sensSlider.value = sensManager.sensValue;

        fullToggle.onValueChanged.AddListener(delegate { OnFullscreenChanged(); });
        fullToggle.isOn = !fullManager.isFullscreenValue;
#if UNITY_WEBGL
        fullToggle.gameObject.transform.parent.gameObject.SetActive(false);
#endif

        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChanged(); });
        SetResolutions();
    }

    void SetResolutions() {
        Resolution[] resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) {
            string optionText = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(new TMP_Dropdown.OptionData(optionText));

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.options = options;

        // Set the initial selected index
        resolutionDropdown.value = currentResolutionIndex;
    }

    public void OnVolumeChanged() {
        volumeManager.OnVolumeChanged(volumeSlider.value);
    }

    public void OnFovChanged() {

        fovManager.OnFovChanged(fovSlider.value);
    }

    public void OnSensChanged() {
        sensManager.OnSensChanged(sensSlider.value);
    }

    public void OnFullscreenChanged() {
        fullManager.OnFullscreenToggle(fullToggle.isOn);
    }

    public void OnResolutionChanged() {
        resolutionManager.OnResolutionChange(resolutionDropdown.value);
    }
}
