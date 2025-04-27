using UnityEngine;

public class PlayerMissles : MonoBehaviour
{
    public GameObject missilePrefab;
    public Transform launchPoint;
    [SerializeField] CameraShake cameraShake;

    [SerializeField] float duration = 0.1f;
    [SerializeField] Vector3 positionPunch = Vector3.one * 0.1f;
    [SerializeField] Vector3 rotationPunch = Vector3.one * 0.15f;
    public int ammo = 0;
    public int collected;
    public int requiredValue = 10;

    public void addAmmo(int value) {
        collected += value;

        if(collected >= requiredValue){
            Debug.Log("Collected missle");
            collected -= requiredValue;
            ammo++;
        }
    }

    void Awake()
    {
        cameraShake = GetComponent<CameraShake>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse2) && ammo > 0){
            var shot = Instantiate(missilePrefab, launchPoint.position, launchPoint.rotation);
            cameraShake.PunchScreen(duration, positionPunch, rotationPunch);
            ammo--;
        }
    }
}
