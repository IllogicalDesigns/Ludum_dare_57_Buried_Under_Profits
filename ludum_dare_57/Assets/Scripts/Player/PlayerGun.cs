using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    bool isPaused;
    public float maxDist = 100f;
    public int baseDamage = 20;
    public int minExtra = 5;
    public int maxExtra = 10;

    [SerializeField] AudioSource gunSfx;
    [SerializeField] AudioSource hitMarkerSfx;

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
                var damage = baseDamage + Random.Range(minExtra, maxExtra);
                hit.collider.SendMessage(Health.OnHitString, new DamageInstance(damage, 0), SendMessageOptions.DontRequireReceiver);

                if(hit.collider.TryGetComponent<Health>(out Health hp)) {
                    hitMarkerSfx.Play();
                }
            }
            else {
                Debug.Log("Missed");
            }
        }
    }
}
