using UnityEngine;

public class HullHealthEffects : MonoBehaviour
{
    public Health hullHealth;

    public Material window;
    public Material crackedWindow1;
    public Material crackedWindow2;
    public Material missingWindow3;
    public Renderer windowRenderer;

    public AudioClip crack;
    bool cracked1;
    bool cracked2;
    bool missing3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        hullHealth = GetComponent<Health>();
        windowRenderer.material = window;
    }

    // Update is called once per frame
    void Update()
    {
        if(hullHealth.hp <= 0) {
            SetMissingWindow();
        }
        else if(hullHealth.hp < 33) {
            SetHeavyCrack2();
        }
        else if(hullHealth.hp < 66) {
            SetCrack1();
        }
        else {
            windowRenderer.material = window;
        }
    }

    private void SetCrack1() {
        windowRenderer.material = crackedWindow1;

        if (!cracked1) {
            AudioManager.instance.PlaySoundOnPlayer(crack);
            cracked1 = true;
        }
    }

    private void SetHeavyCrack2() {
        windowRenderer.material = crackedWindow2;

        if (!cracked2) {
            AudioManager.instance.PlaySoundOnPlayer(crack);
            cracked2 = true;
        }
    }

    private void SetMissingWindow() {
        windowRenderer.material = missingWindow3;

        if (!missing3) {
            AudioManager.instance.PlaySoundOnPlayer(crack);
            missing3 = true;
        }
    }
}
