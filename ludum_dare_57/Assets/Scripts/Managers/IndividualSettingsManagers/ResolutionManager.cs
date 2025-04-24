using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public void OnResolutionChange(int newIndex) {
        Resolution[] resolutions = Screen.resolutions;
        // Assuming your dropdown's options are set up to match the available screen resolutions.
        if (newIndex >= 0 && newIndex < resolutions.Length) {
            Screen.SetResolution(resolutions[newIndex].width, resolutions[newIndex].height, Screen.fullScreen);
            Debug.Log("Changed Resolution to " + resolutions[newIndex].width + "x" + resolutions[newIndex].height + " " + Screen.width + "x" + Screen.height);
            
        }
    }
}
