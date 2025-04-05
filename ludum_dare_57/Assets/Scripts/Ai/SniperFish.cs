using UnityEngine;

public class SniperFish : MonoBehaviour
{
    public float activationDistance = 15f;
    Player player;
    Health playerHealth;
    Transform attackPoint;

    LineRenderer lineRenderer;

    public float timeBeforeSnipeLands = 2f;
    public float cooldown = 10f;
    float timer;

    public int damage = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackPoint = GameObject.Find("AttackPoint").transform;

        lineRenderer = GetComponent<LineRenderer>();
        player = FindAnyObjectByType<Player>();
        playerHealth = player.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > timeBeforeSnipeLands) {
            lineRenderer.enabled = false;
            timer -= Time.deltaTime;  //We are in cooldown
            return;
        }


        if (Vector3.Distance(transform.position, attackPoint.position) < activationDistance) {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, attackPoint.position);

            timer -= Time.deltaTime;
            if (timer < 0) {
                lineRenderer.enabled = false;
                playerHealth.OnHit(damage);
                timer = cooldown;
            }
        }
        else {
            lineRenderer.enabled = false;
            timer = timeBeforeSnipeLands;
        }
    }

    //Can we snipe
    //Are we in range
    //Can the player see us?
    //Can we see the player?
    //Is there an avaliable snipe slot?

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, attackPoint.position);
    }
}
