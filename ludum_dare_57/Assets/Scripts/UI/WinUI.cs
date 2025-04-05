using TMPro;
using UnityEngine;

public class WinUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] string preText = "You collected ";
    [SerializeField] string postText = " gems";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(GameManager.instance.gemCount == 0) {
            winText.text = "You returned with just your life";
        } else {
            winText.text = preText + GameManager.instance.gemCount.ToString() + postText;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
