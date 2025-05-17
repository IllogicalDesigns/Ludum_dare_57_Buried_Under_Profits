using System;
using UnityEngine;

public class HitMarkerUI : MonoBehaviour
{
    public GameObject hitMarker;
    public float timeOfHit = 0.3f;
    PlayerGunEffects gun;

    private void Reset() {
        hitMarker = transform.GetChild(0).gameObject;
    }

    private void Start() {
        gun = FindAnyObjectByType<PlayerGunEffects>();
        gun.OnHitMarker += OnHitMarker;
        HideHitMarker();
    }

    private void OnDestroy() {
        gun.OnHitMarker -= OnHitMarker;
    }

    private void OnHitMarker() {
        hitMarker.SetActive(true);
        Invoke(nameof(HideHitMarker), timeOfHit);
        
    }

    public void HideHitMarker() {
        hitMarker.SetActive(false);
    }
}
