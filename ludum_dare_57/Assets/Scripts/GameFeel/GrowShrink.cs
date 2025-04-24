using UnityEngine;
using DG.Tweening;

public class GrowShrink : MonoBehaviour
{
    [SerializeField] Vector3 maxScale = Vector3.one * 10;
    [SerializeField] Vector3 minScale;
    [SerializeField] float totalDuration = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = minScale;
        transform.DOScale(maxScale, totalDuration / 2).OnComplete(() => { transform.DOScale(minScale, totalDuration / 2); });
    }
}
