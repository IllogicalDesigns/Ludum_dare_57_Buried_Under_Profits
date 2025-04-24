using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public GameObject playerSound;
    public GameObject spatialSound;
    Transform player;

    private void Awake() {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Player>().transform;
    }

    public void PlaySoundOnPlayer(AudioClip clip) {
        var audioSource = Instantiate(playerSound, player.position, player.rotation, player).GetComponent<AudioSource>(); //TODO pool this
        audioSource.clip = clip;
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }

    public void PlaySoundOnPoint(AudioClip clip, Vector3 point) {
        var audioSource = Instantiate(playerSound, player.position, player.rotation, player).GetComponent<AudioSource>();  //TODO pool this
        audioSource.clip = clip;
        audioSource.Play();
        Destroy(audioSource.gameObject, clip.length);
    }
}
