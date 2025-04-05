using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    bool isPaused;
    public float maxDist = 100f;
    public int damage = 25;

    [SerializeField] AudioSource gunSfx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void SetPaused(bool paused) {
        if (paused) {
            isPaused = true;
        }
        else {
            isPaused = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isPaused) { return; }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0)) {
            gunSfx.Play();
            if (Physics.Raycast(ray, out hit, maxDist)) {
                Debug.Log("Hit: " + hit.transform.name);
                hit.collider.SendMessage(Health.OnHitString, damage, SendMessageOptions.DontRequireReceiver);
            }
            else {
                Debug.Log("Missed");
            }
        }
    }
}
