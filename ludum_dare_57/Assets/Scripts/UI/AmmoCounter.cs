using DG.Tweening;
using TMPro;
using UnityEngine;

public class AmmoCounter : MonoBehaviour {
    [SerializeField] TextMeshProUGUI bulletText;
    [SerializeField] string preText = "Bullets: ";
    [Space]
    [SerializeField] int pulseThreshold = 5;
    [Space]
    [SerializeField] float pulseTime = 0.25f;
    [SerializeField] float pulseScale = 1.4f;

    Tween tween;

    PlayerGun playerGun;

    [SerializeField] float bounceStrength = 0.1f;
    [SerializeField] float bounceDuration = 0.75f;
    [Space]
    [SerializeField] string outOfBullets = "left shift?";

    Vector3 origScale;

    [Space]
    public Color originalColor = Color.white;
    public Color flashGoodColor = Color.green;
    public float flashDuration = 0.4f;
    bool isFlashing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        playerGun = FindAnyObjectByType<PlayerGun>();
        origScale = bulletText.transform.localScale;
        PlayerGun.GunFiredEvent += UpdateAmmo;
        PlayerGun.AmmoAddedEvent += UpdateAmmo;
        PlayerGun.OutOfAmmoEvent += UpdateAmmo;
        UpdateAmmo();

        originalColor = bulletText.color;
    }

    void OnDestroy() {
        PlayerGun.GunFiredEvent -= UpdateAmmo;
        PlayerGun.AmmoAddedEvent -= UpdateAmmo;
        PlayerGun.OutOfAmmoEvent -= UpdateAmmo;
    }

    // Update is called once per frame
    void UpdateAmmo() {
        var ammo = playerGun.ammo;

        if (tween == null && ammo < pulseThreshold)
            tween = bulletText.transform.DOShakeScale(pulseTime, pulseScale).SetLoops(-1, LoopType.Yoyo);

        if (tween != null && ammo > pulseThreshold) {
            tween.Kill(true);
            bulletText.transform.localScale = origScale;
            tween = null;
        }

        if (bulletText != null) {
            if (ammo <= 0)
                bulletText.text = outOfBullets;
            else
                bulletText.text = preText + ammo.ToString();
        }
    }

    public void ProvidedAmmo() {
        bulletText.transform.DOKill(true);
        bulletText.transform.DOPunchScale(transform.localScale * bounceStrength, bounceDuration);
        FlashText();
    }

    public void FlashText() {
        bulletText.color = flashGoodColor;
        isFlashing = true;
        Invoke(nameof(RevertText), flashDuration);
    }

    public void RevertText() {
        bulletText.color = originalColor;
        isFlashing = false;
    }
}

