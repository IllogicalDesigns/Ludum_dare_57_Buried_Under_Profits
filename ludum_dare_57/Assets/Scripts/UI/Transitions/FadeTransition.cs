using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class FadeTransition : Transition {
    [SerializeField] Image fadeImage;
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] float duration = 1f;
    const float OUTALPHA = 1f;
    const float INALPHA = 0f;
    const float TXTSPDMOD = 0.5f;

    private void SetEnabled(bool isEnabled) {
        loadingText.gameObject.SetActive(isEnabled);
        fadeImage.enabled = isEnabled;
    }

    public override void PlayOutTransition() {
        SetEnabled(true);

        loadingText.DOFade(OUTALPHA, duration * TXTSPDMOD);
        fadeImage.DOFade(OUTALPHA, duration).OnComplete(() => { onBetweenEvent.Invoke(); OnComplete(); });
    }

    public override void PlayInTransition() {
        SetEnabled(true);

        loadingText.DOFade(INALPHA, duration * TXTSPDMOD);
        fadeImage.DOFade(INALPHA, duration).OnComplete(() => {
            OnCompleteEvent.Invoke();
            SetEnabled(false);
            OnComplete();
        });
    }

    public override void OnComplete() {
        TransitionManager.Instance.TransitionCompleted();
    }
}
