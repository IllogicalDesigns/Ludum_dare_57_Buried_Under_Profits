using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AirSupplyHealth : MonoBehaviour
{
    [SerializeField] AudioSource hardToBreath1;
    [SerializeField] AudioClip breathOfFreshAir;
    [SerializeField] float hardToBreath1Thresh = 15f;
    [Space]
    [SerializeField] AudioSource hardToBreath2;
    [SerializeField] AudioClip coughSomeAir;
    [SerializeField] float hardToBreath2Thresh = 7f;
    [Space]
    [SerializeField] Image vignette;
    [SerializeField] float dur = 0.2f;
    Tween tween;

    [SerializeField] AudioSource airCritical;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vignette = GameObject.Find("Vingette").GetComponent<Image>();
        vignette.DOFade(0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        float air = GameManager.instance.air;

        if(air <= 0) {
            hardToBreath1.Stop();
            hardToBreath2.Stop();
            vignette.DOKill(true);
        }
        else if(air < hardToBreath2Thresh) {
            if(hardToBreath1.isPlaying) hardToBreath1.Stop();
            if(!airCritical.isPlaying) airCritical.Play();

            if (tween == null) {
                vignette.DOKill(true);
                tween = vignette.DOFade(1f, dur).SetLoops(-1, LoopType.Yoyo);
            }

            if (!hardToBreath2.isPlaying) {
                hardToBreath2.Play();
            }
        }
        else if (air < hardToBreath1Thresh) {
            if (hardToBreath2.isPlaying) {
                hardToBreath2.Stop();
                AudioManager.instance.PlaySoundOnPlayer(coughSomeAir);
            }

            if (!hardToBreath1.isPlaying) {
                hardToBreath1.Play();
            }

            vignette.DOKill(true);
            vignette.DOFade(0f, 0f);
            tween = null;
        } else {
            if (hardToBreath2.isPlaying) hardToBreath2.Stop();

            if (hardToBreath1.isPlaying) {
                hardToBreath1.Stop();
                AudioManager.instance.PlaySoundOnPlayer(breathOfFreshAir);
            }

            vignette.DOKill(true);
            vignette.DOFade(0f, 0f);
            tween = null;
        }
    }

    public void OnDead() {
        vignette.DOKill(true);
        vignette.DOFade(1f, 0f);
        tween = null;
    }

    public void OnHit(DamageInstance damageInstance) {
        GameManager.instance.DamageAir(damageInstance.airDamage);
    }
}
