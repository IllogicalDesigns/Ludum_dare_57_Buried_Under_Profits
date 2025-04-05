using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIMouseAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
    [SerializeField] Vector3 scaleSize = Vector3.one * 1.1f;
    [SerializeField] Vector3 clickHoldScale = Vector3.one * 0.9f;
    [SerializeField] float duration = 0.15f;
    [SerializeField] Ease ease = Ease.InOutCirc;
    Vector3 originalScale;

    [Space] 
    [SerializeField] Vector3 punchScaleSize = Vector3.one;
    [SerializeField] float punchDuration = 0.15f;

    // Start is called before the first frame update
    void Awake() {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update() {
    }

    private void OnEnable() {
        transform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        transform.DOScale(scaleSize, duration).SetEase(ease);
    }


    public void OnPointerDown(PointerEventData eventData) {
        transform.DOPunchScale(punchScaleSize, punchDuration);
        transform.DOScale(clickHoldScale, duration).SetEase(ease);
    }


    public void OnPointerExit(PointerEventData eventData) {
        transform.DOScale(originalScale, duration).SetEase(ease);
    }
}
