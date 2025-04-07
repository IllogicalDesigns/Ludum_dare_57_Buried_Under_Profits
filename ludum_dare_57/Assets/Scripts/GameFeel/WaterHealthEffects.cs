using UnityEngine;

public class WaterHealthEffects : MonoBehaviour
{
    [SerializeField] Health healthScript;
    [Space]
    [SerializeField] AudioSource waterDripping;
    [SerializeField] float minPitch = 1f;
    [SerializeField] float maxPitch = 1.5f;
    [SerializeField] int bufferBeforeDrip = 10;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.DownArrow))
            healthScript.hp -= 5;

        if (healthScript == null || waterDripping == null)
            return;

        if (!waterDripping.isPlaying && healthScript.hp < healthScript.maxHp- bufferBeforeDrip) {
            waterDripping.Play();
        }

        // Smooth proportional pitch based on health
        float currentHp = Mathf.Max(healthScript.hp, 0f); // avoid negative hp
        float maxHp = Mathf.Max(healthScript.maxHp, 1f);   // avoid divide by zero
        float hpRatio = currentHp / maxHp;                 // 1 = full, 0 = dead

        // Optional: smooth the ramp so it's more gradual
        float curvedRatio = Mathf.Pow(1f - hpRatio, 1.5f); // gentle at first, steeper near low health
        float pitch = Mathf.Lerp(minPitch, maxPitch, curvedRatio);

        waterDripping.pitch = pitch;
    }
}
