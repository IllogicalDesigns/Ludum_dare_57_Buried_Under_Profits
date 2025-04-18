using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health hp;
    public Slider slider;
    public Slider underSlider;
    float currentVelocity;
    public float underSmoothingTime = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.maxValue = hp.maxHp;
        slider.value = hp.hp;

        underSlider.maxValue = hp.maxHp;
        underSlider.value = hp.hp;
    }

    // Update is called once per frame
    void Update()
    {    
        slider.value = hp.hp;

        underSlider.value = Mathf.SmoothDamp(underSlider.value, slider.value, ref currentVelocity, underSmoothingTime);

        //if (underSlider.value > slider.value)
        //    underSlider.value -= Time.deltaTime;
    }
}
 