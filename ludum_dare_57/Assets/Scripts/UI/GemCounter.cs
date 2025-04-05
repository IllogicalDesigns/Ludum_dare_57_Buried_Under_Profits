using TMPro;
using UnityEngine;

public class GemCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gemText;
    [SerializeField] string preText = "gem count: ";

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
}
