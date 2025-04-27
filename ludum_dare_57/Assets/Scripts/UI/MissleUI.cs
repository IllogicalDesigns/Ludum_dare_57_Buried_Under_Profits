using TMPro;
using UnityEngine;

public class MissleUI : MonoBehaviour
{
    public PlayerMissles playerMissles;
    public TextMeshProUGUI text;
    public string pretext = "Missles: ";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMissles = FindFirstObjectByType<PlayerMissles>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text =  pretext + playerMissles.ammo.ToString();
    }
}
