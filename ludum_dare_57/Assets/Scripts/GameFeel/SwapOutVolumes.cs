using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class SwapOutVolumes : MonoBehaviour
{
    public Volume underwater;
    public Volume aboveWater;
    public float waterLineHeight = 40;
    Transform player;

    bool controlFog = true;
    bool controlSkybox = true;

    bool firstTimeEnter = true;

    public AudioClip dive;
    public AudioClip surface;
    Tween tween;

    public Vector3 rotation;
    public float rotationDuration = 10f;
    public Ease ease = Ease.Linear;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>().transform;
        tween = player.DOBlendableLocalRotateBy(rotation, rotationDuration).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
    }

    // Update is called once per frame
    void Update()
    {

        //TODo make this fire once
        if(player.transform.position.y < waterLineHeight) {
            underwater.gameObject.SetActive(true);
            aboveWater.gameObject.SetActive(false);
            if(controlFog) 
                RenderSettings.fog = true;

            if(controlSkybox) 
                Camera.main.clearFlags = CameraClearFlags.Color;

            if(firstTimeEnter){
                firstTimeEnter = false;
                GameObject.Find("BubbleEnter Particle System").GetComponent<ParticleSystem>().Play();
                AudioManager.instance.PlaySoundOnPlayer(dive);
                tween.Kill();
                tween = null;
            }
        } else {
            underwater.gameObject.SetActive(false);
            aboveWater.gameObject.SetActive(true);
            if (controlFog) 
                RenderSettings.fog = false;

            if(controlSkybox) 
                Camera.main.clearFlags = CameraClearFlags.Skybox;

            if(!firstTimeEnter){
                firstTimeEnter = true;
                GameObject.Find("BubbleExit Particle System").GetComponent<ParticleSystem>().Play();
                // AudioManager.instance.PlaySoundOnPlayer(surface);
                if(tween == null)
                    tween = player.DOBlendableLocalRotateBy(rotation, rotationDuration).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
            }
        }
    }
}
