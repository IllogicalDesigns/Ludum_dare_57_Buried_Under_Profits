using UnityEngine;
using UnityEngine.Rendering;

public class SwapOutVolumes : MonoBehaviour
{
    public Volume underwater;
    public Volume aboveWater;
    public float waterLineHeight = 40;
    Transform player;

    bool controlFog = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y < waterLineHeight) {
            underwater.gameObject.SetActive(true);
            aboveWater.gameObject.SetActive(false);
            if(controlFog) RenderSettings.fog = true;
        } else {
            underwater.gameObject.SetActive(false);
            aboveWater.gameObject.SetActive(true);
            if (controlFog) RenderSettings.fog = false;
        }
    }
}
