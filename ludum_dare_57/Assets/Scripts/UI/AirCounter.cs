using DG.Tweening;
using TMPro;
using UnityEngine;

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

    Tween tween;

    [Space]
    [SerializeField] GameObject airLostText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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

        if (tween == null && air < pulseThreshold)
            tween = airText.transform.DOShakeScale(pulseTime, pulseScale).SetLoops(-1, LoopType.Yoyo);

        if (tween != null && air > pulseThreshold) {
            tween.Kill(true);
            airText.transform.localScale = Vector3.one;
            tween = null;
        }

        if (airText != null)
            airText.text = colorText + preText + air.ToString(Format) + postText;
    }

    public void DamagedAir(int value) {
        var newText = Instantiate(airLostText, airText.transform.parent);
        newText.GetComponent<TextMeshProUGUI>().text = "-" + value.ToString("D");
        //newText.transform.DOScale(Vector3.zero, 3f);
            //newText.transform.DOPunchScale(transform.localScale * 1.1f, 1f).OnComplete(() => { 
            //    newText.transform.DOScale(Vector3.zero, 1f).OnComplete(() => { 
            //    newText.transform.DOKill(); 
            //    Destroy(newText);
            //}); });
        }
}
