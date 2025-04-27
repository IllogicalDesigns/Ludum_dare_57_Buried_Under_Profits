using UnityEngine;

public class ShakeItOffUI : MonoBehaviour
{
    public GameObject visuals;

    public void ShakeItOff(bool isVisible){
        visuals.SetActive(isVisible);
    }
}
