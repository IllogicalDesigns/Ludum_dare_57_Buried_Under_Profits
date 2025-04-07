using UnityEngine;

public class BubbleLoudnessBasedOnSpeed : MonoBehaviour
{
    CharacterController characterController;
    [SerializeField] float minVolume = 0.025f;
    [SerializeField] float maxVolume = 0.5f;
    AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = FindAnyObjectByType<Player>().GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        var magnitude = characterController.velocity.magnitude;

        audioSource.volume = Mathf.Clamp(magnitude, minVolume, maxVolume);
    }
}
