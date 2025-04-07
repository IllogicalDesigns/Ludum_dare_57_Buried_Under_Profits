using DG.Tweening;
using TMPro;
using UnityEngine;

public class GemCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gemText;
    [SerializeField] string preText = "gem count: ";
    [SerializeField] float bounceStrength = 0.1f;
    [SerializeField] float bounceDuration = 0.75f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gemText != null) 
            gemText.text = preText +  GameManager.instance.gemCount.ToString();
    }

    public void BounceCounter() {
        gemText.transform.DOKill(true);
        gemText.transform.DOPunchScale(transform.localScale * bounceStrength, bounceDuration);
    }
}
