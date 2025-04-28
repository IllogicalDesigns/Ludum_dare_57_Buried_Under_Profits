using System.Collections.Generic;
using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    public TriggerBubbleAmmo ammoBubble;
    public TriggerHullBubble hullBubble;
    public TriggerBubbleAdd airBubble;

    public void OnDead() {
        // Drop selectedDrop = drops[UnityEngine.Random.Range(0, drops.Count)];
        var playerGun = FindAnyObjectByType<PlayerGun>();

        // Define the maximum values
        const int maxAmmo = 10;
        const int maxHp = 100;
        const int maxAir = 45;

        var ammo = playerGun.ammo;
        var hull = playerGun.GetComponent<Health>().hp;
        var air = GameManager.instance.air;

        var ammoProportion = (float)ammo / maxAmmo;
        var hullProportion = (float)hull / maxHp;
        var airProportion = (float)air / maxAir;

        GameObject selectedDrop = airBubble.gameObject;
        // Determine the lowest proportion and spawn the corresponding bubble
        if (ammoProportion <= hullProportion && ammoProportion <= airProportion)
        {
            selectedDrop = ammoBubble.gameObject;
        }
        else if (hullProportion <= ammoProportion && hullProportion <= airProportion)
        {
            selectedDrop = hullBubble.gameObject;
        }
        else
        {
            selectedDrop = airBubble.gameObject;
        }

        Instantiate(selectedDrop, transform.position, Quaternion.identity); 
    }
}
