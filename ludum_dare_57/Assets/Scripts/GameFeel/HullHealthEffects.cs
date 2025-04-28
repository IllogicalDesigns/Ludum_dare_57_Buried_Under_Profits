using DG.Tweening;
using TMPro;
using UnityEngine;

public class HullHealthEffects : MonoBehaviour
{
    public Health hullHealth;

    public Material window;
    public Material crackedWindow1;
    public Material crackedWindow2;
    public Material missingWindow3;
    public Renderer windowRenderer;

    [Space]
    public AudioClip crack;
    bool cracked1;
    bool cracked2;
    bool missing3;

    [Space]
    public ParticleSystem waterParticles2;
    public ParticleSystem waterParticles1;

    [Space]
    public TextMeshProUGUI criticalText;
    Tween criticalTween;
    [SerializeField] Vector3 punchScale = Vector3.one * 0.3f;
    [SerializeField] float duration = 0.5f;
    [SerializeField] AudioSource criticalAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        hullHealth = GetComponent<Health>();
        windowRenderer.material = window;
        criticalText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1)) {
            hullHealth.hp = 19;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2)) {
            hullHealth.hp = 32;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3)) {
            hullHealth.hp = 65;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4)) {
            hullHealth.hp = 100;
        }

        var hp = hullHealth.hp;

        if (hp <= 0) {
            SetMissingWindow();
            if (waterParticles2.isPlaying) waterParticles2.Stop();
            if (waterParticles1.isPlaying) waterParticles1.Stop();
        }
        else if (hp < 20) {
            if (criticalTween == null) {
                criticalText.gameObject.SetActive(true);
                criticalText.transform.DOKill(true);
                criticalTween = criticalText.transform.DOPunchScale(punchScale, duration).OnComplete(() => criticalTween = null);
            }

            if (!criticalAudio.isPlaying) 
                criticalAudio.Play();
        }
        else if(hp < 33) {
            SetHeavyCrack2();
            if(!waterParticles2.isPlaying) waterParticles2.Play();
            criticalText.gameObject.SetActive(false);
            if (criticalTween == null) { 
                criticalTween.Kill();
                 criticalTween = null;
            }
        }
        else if(hp < 66) {
            SetCrack1();
            if (!waterParticles1.isPlaying) waterParticles1.Play();
            if (waterParticles2.isPlaying) waterParticles2.Stop();
            criticalText.gameObject.SetActive(false);
            if (criticalTween == null) { 
                criticalTween.Kill();
                 criticalTween = null;
            }
        }
        else {
            if (waterParticles2.isPlaying) waterParticles2.Stop();
            if (waterParticles1.isPlaying) waterParticles1.Stop();
            windowRenderer.material = window;
            criticalText.gameObject.SetActive(false);
            if (criticalTween == null) { 
                criticalTween.Kill();
                 criticalTween = null;
            }
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
