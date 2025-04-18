using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DodgeBar : MonoBehaviour {
    public Player player;
    public Slider slider;

    [SerializeField] Vector3 punchScale = Vector3.one * 0.1f;
    [SerializeField] float duration = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        slider.maxValue = player.dodgeTimer;
    }

    // Update is called once per frame
    void Update() {
        slider.value = player.dgTimer;

        if (slider.value < slider.maxValue && Input.GetKeyDown(KeyCode.LeftShift)) {
            slider.transform.DOKill(true);
            slider.transform.DOPunchScale(punchScale, duration);
        }
    }
}

