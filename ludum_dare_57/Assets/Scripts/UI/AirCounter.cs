using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirCounter : MonoBehaviour
{
    private const string Format = "F1";
    [SerializeField] TextMeshProUGUI airText;
    [SerializeField] string preText = "Air left: ";
    [SerializeField] string postText = " seconds";
    [Space]
    [SerializeField] float yellowThreshold = 0.5f;
    [SerializeField] float redThreshold = 0.25f;
    [SerializeField] float pulseThreshold = 0.1f;
    [Space]
    [SerializeField] float pulseTime = 0.25f;
    [SerializeField] float pulseScale = 1.4f;

    public Slider slider;
    public Slider underSlider;

    Vector3 origanlScale;

    Tween tween;

    [Space]
    [SerializeField] GameObject airLostText;

    [SerializeField] float bounceStrength = 0.1f;
    [SerializeField] float bounceDuration = 0.75f;

    float currentVelocity;
    public float underSmoothingTime = 1f;

    [Space]
    public Color originalColor = Color.white;
    public Color flashGoodColor = Color.green;
    public Color flashBadColor = Color.red;
    public float flashDuration = 0.1f;
    bool isFlashing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origanlScale = transform.localScale;

        var air = GameManager.instance.air;
        slider.maxValue = air;
        slider.value = air;

        underSlider.maxValue = air;
        underSlider.value = air;

        originalColor = airText.color;
    }

    // Update is called once per frame
    void Update()
    {
        var air = GameManager.instance.air;
        var colorText = "";

        if(air < redThreshold)
            colorText = "<color=\"red\">";
        else if (air < yellowThreshold)
            colorText = "<color=\"yellow\">";

        if (isFlashing) colorText = "";

        if (tween == null && air < pulseThreshold)
            tween = airText.transform.DOShakeScale(pulseTime, pulseScale).SetLoops(-1, LoopType.Yoyo);

        if (tween != null && air > pulseThreshold) {
            tween.Kill(true);
            airText.transform.localScale = origanlScale;
            tween = null;
        }

        if (airText != null)
            airText.text = colorText + preText + air.ToString(Format) + postText;

        slider.value = air;

        underSlider.value = Mathf.SmoothDamp(underSlider.value, slider.value, ref currentVelocity, underSmoothingTime);

        //if (underSlider.value > slider.value)
        //    underSlider.value -= Time.deltaTime * 0.9f;

        //if (underSlider.value < slider.value)
        //    underSlider.value = slider.value;
    }

    public void DamagedAir(int value) {
        airText.transform.DOKill(true);
        airText.transform.DOPunchScale(transform.localScale * bounceStrength, bounceDuration);
        airText.color = flashBadColor;
        isFlashing = true;
        Invoke(nameof(RevertText), flashDuration);
    }

    public void ProvidedAir(int value) {
        airText.transform.DOKill(true);
        airText.transform.DOPunchScale(transform.localScale * bounceStrength, bounceDuration);
        airText.color = flashGoodColor;
        isFlashing = true;
        Invoke(nameof(RevertText), flashDuration);
    }

    public void RevertText() {
        airText.color = originalColor;
        isFlashing = false; 
    }
}
