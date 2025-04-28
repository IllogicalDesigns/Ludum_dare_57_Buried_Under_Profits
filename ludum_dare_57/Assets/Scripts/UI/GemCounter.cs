using DG.Tweening;
using TMPro;
using UnityEngine;

public class GemCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gemText;
    [SerializeField] string preText = "gem count: ";
    [SerializeField] float bounceStrength = 0.1f;
    [SerializeField] float bounceDuration = 0.75f;

    void Start() {
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
        }
    }

    public void BounceCounter() {
        gemText.transform.DOKill(true);
        gemText.transform.DOPunchScale(transform.localScale * bounceStrength, bounceDuration);
    }
}
