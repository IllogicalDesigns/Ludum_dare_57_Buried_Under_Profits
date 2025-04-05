using UnityEngine;
using UnityEngine.UI;

public class AltitudeBar : MonoBehaviour
{
    public float min = 0f;
    public float max = 0f;
    [SerializeField] Slider slider;
    Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = FindAnyObjectByType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = playerTransform.position.y;
    }
}
