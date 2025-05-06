using DG.Tweening;
using TMPro;
using UnityEngine;

public class GemCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gemText;
    [SerializeField] string preText = "gem count: ";
    [SerializeField] float bounceStrength = 0.1f;
    [SerializeField] float bounceDuration = 0.75f;

    [Space]
    public Color originalColor = Color.white;
    public Color flashGoodColor = Color.green;
    public float flashDuration = 0.1f;
    bool isFlashing;

    void Start() {
        originalColor = gemText.color;
        GameManager.GemCollectedEvent += UpdateGemCounter;
    }

    void OnDestroy() {
        GameManager.GemCollectedEvent -= UpdateGemCounter;
    }

    // Update is called once per frame
    void UpdateGemCounter()
    {
        if(gemText != null) {
            gemText.text = preText +  GameManager.instance.gemCount.ToString();
            BounceCounter();
            FlashText();
        }
    }

    public void BounceCounter() {
        gemText.transform.DOKill(true);
        gemText.transform.DOPunchScale(transform.localScale * bounceStrength, bounceDuration);
    }

    public void FlashText() {
        gemText.color = flashGoodColor;
        isFlashing = true;
        Invoke(nameof(RevertText), flashDuration);
    }

    public void RevertText() {
        gemText.color = originalColor;
        isFlashing = false;
    }
}
