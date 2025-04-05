using TMPro;
using UnityEngine;

public class AirCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI airText;
    [SerializeField] string preText = "Air left: ";
    [SerializeField] string postText = " seconds";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (airText != null)
            airText.text = preText + GameManager.instance.air.ToString("F2") + postText;
    }
}
