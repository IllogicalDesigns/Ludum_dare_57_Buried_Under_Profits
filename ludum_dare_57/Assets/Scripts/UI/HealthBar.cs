using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health hp;
    public Slider slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = hp.hp;
        slider.maxValue = hp.maxHp;
    }
}
