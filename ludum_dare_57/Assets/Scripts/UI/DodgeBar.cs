using UnityEngine;
using UnityEngine.UI;

public class DodgeBar : MonoBehaviour {
    public Player player;
    public Slider slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        slider.maxValue = player.dodgeTimer;
    }

    // Update is called once per frame
    void Update() {
        slider.value = player.dgTimer;
    }
}

