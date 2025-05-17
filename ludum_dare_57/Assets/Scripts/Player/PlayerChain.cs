using DG.Tweening;
using UnityEngine;

public class PlayerChain : MonoBehaviour
{
    public static event System.Action LaunchEvent;
    public static event System.Action LatchEvent;
    public static event System.Action impactEvent;

    LineRenderer lineRenderer;
    float maxDist = Mathf.Infinity;
    public LayerMask layerMask = ~0;
    public bool chaining;
    public Transform chainTarget;
    public Transform chainStart;
    public GameObject chainModel;
    public CharacterController controller;
    public Collider hitCollider;
    public float chainSpeed = 5f; 
    public float impactDistance = 3f;
    public int impactDamage = 25;
    KeyCode chainCode = KeyCode.R;

    //CameraShake cameraShake;

    Health health;
    public float impactInvulTime = 0.1f;

    public float chainLaunchTime = 0.25f;

    float chainTime;
    RaycastHit chainHit;
    public float launchSpeed = 5f;

    Vector3 lastPos;

    public enum ChainState {
        idle,
        launching,
        pulling,
        retracting
    }

    public ChainState chainState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = GetComponentInParent<Health>();
        //cameraShake = FindAnyObjectByType<CameraShake>();
        lineRenderer = GetComponent<LineRenderer>();

        chainModel?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(chainState == ChainState.idle && Input.GetKeyDown(chainCode)) {
            LaunchChain();
        }

        if (chainState == ChainState.launching && Input.GetKey(chainCode)) {
            ExtendChain();
        }

        if (chainState == ChainState.pulling && chainTarget != null && Input.GetKey(chainCode)) {
            PullInChainAndSub();
        }

        if (chainState != ChainState.idle && Input.GetKeyUp(chainCode)) {
            StopChain();
        }

        if(chainState == ChainState.retracting) {
            RetractChain();
        }
    }

    private void ExtendChain() {
        chainTime += launchSpeed * Time.deltaTime;
        chainTarget.position = Vector3.Lerp(chainStart.position, chainHit.point, chainTime);

        UpdateVisibleChain();

        if (Vector3.Distance(chainTarget.position, chainHit.point) < impactDistance | chainTime > 1) {
            StartRetractChain(chainHit);
        }
    }

    private void RetractChain() {
        chainTime += launchSpeed * Time.deltaTime;
        chainTarget.position = Vector3.Lerp(lastPos, chainStart.position, chainTime);

        UpdateVisibleChain();

        if (Vector3.Distance(chainTarget.position, chainStart.position) < impactDistance | chainTime > 1) {
            chainState = ChainState.idle;
            chainModel?.SetActive(false);
            lineRenderer.enabled = false;
            lastPos = chainTarget.position;
            chainTime = 0;
        }
    }

    private void LaunchChain() {
        LaunchEvent?.Invoke();
        chainState = ChainState.launching;
        //cameraShake.PunchScreen();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDist, layerMask)) {
            Debug.Log("Chain Hit: " + hit.transform.name);
            chainTime = 0;
            chainHit = hit;
        }
        else {
            chainState = ChainState.idle;
            Debug.Log("Missed");
        }
    }

    private void PullInChainAndSub() {
        Vector3 chainDirection = GetChainDirection();
        controller.Move(chainDirection.normalized * chainSpeed * Time.deltaTime);

        UpdateVisibleChain();

        //When close stop!  also if we add momentum this will pair nicely
        var dist = Vector3.Distance(transform.position, chainTarget.position);
        if (dist < impactDistance) {
            if (health.canTakeDamage) {
                health.canTakeDamage = false;
                Invoke(nameof(ReallowDamge), impactInvulTime);
            }

            impactEvent?.Invoke();
            hitCollider.SendMessage("OnHit", new DamageInstance(impactDamage, 0, DamageInstance.DamageType.collision));
            //cameraShake.PunchScreen();
            StopChain();
        }
    }

    private void UpdateVisibleChain() {
        lineRenderer.SetPosition(0, chainStart.position);
        lineRenderer.SetPosition(1, chainTarget.position);
        lineRenderer.enabled = true;
    }

    private Vector3 GetChainDirection() {
        return transform.position - chainTarget.position;
    }

    private void StartRetractChain(RaycastHit hit) {
        LatchEvent?.Invoke();
        chainState = ChainState.pulling;
        chaining = true;
        hitCollider = hit.collider;
        chainTarget.position = hit.point;
        chainTarget.SetParent(hit.transform);
        chainModel?.SetActive(true);
    }

    public void ReallowDamge() {
        health.canTakeDamage = true;
    }

    private void StopChain() {
        chainState = ChainState.retracting;
        chainTarget.SetParent(null);
        chaining = false;

        hitCollider = null;


        lastPos = chainTarget.position;
        chainTime = 0;

        chainHit = default;
    }
}
