using UnityEngine;

public class ShakeOnAwake : MonoBehaviour
{
    [SerializeField] float duration = 0.1f;
    [SerializeField] Vector3 positionPunch = Vector3.one;
    [SerializeField] Vector3 rotationPunch = Vector3.one * 15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var cameraShake = FindAnyObjectByType<CameraShake>();
        cameraShake.PunchScreen(duration, positionPunch, rotationPunch);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
