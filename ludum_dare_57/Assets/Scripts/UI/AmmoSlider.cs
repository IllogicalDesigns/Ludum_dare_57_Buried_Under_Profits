using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoSlider : MonoBehaviour {
    [SerializeField] Slider bulletSlider;

    PlayerGun playerGun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        playerGun = FindAnyObjectByType<PlayerGun>();
    }

    // Update is called once per frame
    void Update() {
        var ammo = playerGun.ammo;
        if(ammo > bulletSlider.maxValue) bulletSlider.maxValue = ammo;

        bulletSlider.value = ammo;
    }

    //public void ProvidedAmmo() {
    //}
}


