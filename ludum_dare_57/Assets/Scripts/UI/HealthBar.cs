using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health hp;
    public Image fillImage;
    public Slider slider;
    public Slider underSlider;
    float currentVelocity;
    public float underSmoothingTime = 1f;

    [Space]
    public Color originalColor = Color.white;
    public Color flashGoodColor = Color.green;
    public Color flashBadColor = Color.white;
    public float flashDuration = 0.1f;
    bool isFlashing;

    [Space]
    [SerializeField] float bounceStrength = 0.1f;
    [SerializeField] float bounceDuration = 0.75f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.maxValue = hp.maxHp;
        slider.value = hp.hp;

        underSlider.maxValue = hp.maxHp;
        underSlider.value = hp.hp;

        hp.OnHitEvent += hasBeenHit;
        hp.OnHealEvent += hadBeenHealed;

        originalColor = fillImage.color;
    }

    private void OnDestroy() {
        hp.OnHitEvent -= hasBeenHit;
        hp.OnHealEvent -= hadBeenHealed;
    }

    private void hasBeenHit() {
        FlashText(flashBadColor);
        slider.transform.DOKill(true);
        slider.transform.DOPunchScale(transform.localScale * bounceStrength, bounceDuration);
    }

    private void hadBeenHealed() {
        FlashText(flashGoodColor);
        slider.transform.DOKill(true);
        slider.transform.DOPunchScale(transform.localScale * bounceStrength, bounceDuration);
    }

    public void FlashText(Color color) {
        fillImage.color = color;
        isFlashing = true;
        Invoke(nameof(RevertText), flashDuration);
    }

    public void RevertText() {
        fillImage.color = originalColor;
        isFlashing = false;
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
 