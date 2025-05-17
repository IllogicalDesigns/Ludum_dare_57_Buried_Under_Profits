using System.Collections.Generic;
using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    public TriggerBubbleAmmo ammoBubble;
    public TriggerHullBubble hullBubble;
    public TriggerBubbleAdd airBubble;

    public void OnDead() {
        var playerGun = FindAnyObjectByType<PlayerGun>();
        var player = FindAnyObjectByType<Player>();

        // Define the maximum values
        const int maxAmmo = 10;
        const int maxHp = 100;
        const int maxAir = 45;

        var ammo = playerGun.ammo;
        var hull = player.GetComponent<Health>().hp;
        var air = GameManager.instance.air;

        var ammoProportion = (float)ammo / maxAmmo;
        var hullProportion = (float)hull / maxHp;
        var airProportion = (float)air / maxAir;

        Instantiate(ammoBubble, transform.position, Quaternion.identity); 
        Instantiate(hullBubble, transform.position, Quaternion.identity); 
        Instantiate(airBubble, transform.position, Quaternion.identity); 
    }
}
