using UnityEngine;

public class FOVManager : MonoBehaviour {
    const string fov = "fov";
    public float maxFov = 100f;
    public float minFov = 40f;
    public float defaultFov = 75f;
    public float fovValue;

    void Start() {
        fovValue = PlayerPrefs.GetFloat(fov, defaultFov);
        SetFov(fovValue);
    }

    public void OnFovChanged(float value) {
        Debug.Log($"FOV changed: {value}");
        SetFov(value);
        PlayerPrefs.SetFloat(fov, Mathf.Clamp(value, minFov, maxFov));
        PlayerPrefs.Save();
    }

    void SetFov(float value) {
        //virtualCamera.Lens.FieldOfView = Mathf.Clamp(value, minFov, maxFov);
        Camera.main.fieldOfView = Mathf.Clamp(value, minFov, maxFov); ;
    }
}