using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeInOnStart : MonoBehaviour
{
    public Image image;
    public float fadeTarget = 1f;
    public float fadeTime = 2f;
    public Ease ease = Ease.InOutBounce;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        image.DOFade(fadeTarget, fadeTime).SetEase(ease);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
