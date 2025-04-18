using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SlowMotionUI : MonoBehaviour
{
    SloMotion sloMotion;
    Slider slider;
    public Image vignette;

    [SerializeField] Vector3 punchScale = Vector3.one;
    [SerializeField] float duration = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sloMotion = FindFirstObjectByType<SloMotion>();
        slider = GetComponentInChildren<Slider>();
        slider.minValue = 0f;
        slider.maxValue = sloMotion.maxSlowTime.Evaluate(GameManager.instance.difficulty);
        slider.value = sloMotion.slowTimer;
    }

    // Update is called once per frame
    void Update()
    {
        slider.gameObject.SetActive(!(sloMotion.slowTimer <= 0));
        vignette.gameObject.SetActive(sloMotion.isSlowMotion);
        slider.value = sloMotion.slowTimer;

        if(!(sloMotion.slowTimer <= 0) && Input.GetMouseButtonDown(1)) {
            slider.transform.DOKill(true);
            slider.transform.DOPunchScale(punchScale, duration);
        }
    }
}
