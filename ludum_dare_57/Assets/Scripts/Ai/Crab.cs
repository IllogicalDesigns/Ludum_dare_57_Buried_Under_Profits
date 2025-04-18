using DG.Tweening;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Crab : MonoBehaviour
{
    public float activationDistance = 20f;
    public AudioSource alerted;

    public enum CrabState {
        idle,
        preLaser,
        lasering,
        coolingDown,
    }
    public CrabState state;

    [Space]
    float preLaserTime = 3f;
    public AnimationCurve preLaserTimeAnimationCurve = new AnimationCurve(new Keyframe(0f, 6f), new Keyframe(2, 2f));
    float preLaserTimer;
    public AudioSource laserWindup;
    public float minPitch = 0.7f;
    public float pitchUpSpeed = 0.5f;
    public ParticleSystem windupParticles;
    public float preLaserLookSpeed = 5f;

    [Space]
    public Transform laseringCube;
    public Transform laseringEnd;
    public LineRenderer lineRenderer;
    float laseringTime = 3f;
    public AnimationCurve laseringTimeAnimationCurve = new AnimationCurve(new Keyframe(0f, 2f), new Keyframe(2, 6f));
    float laseringTimer;
    public AudioSource laserFiring;
    public float laserLookSpeed = 5f;
    Tween lasering;
    public LayerMask layerMask;
    public float maxLaserDistance = 40f;

    [Space]
    public Renderer laserHotBitsRender;
    public Material coolMaterial;
    public Material warmMaterial;
    public Material hotMaterial;

    [Space]
    float coolDownTime = 3f;
    public AnimationCurve coolDownTimeAnimationCurve = new AnimationCurve(new Keyframe(0f, 6f), new Keyframe(2, 2f));
    float coolDownTimer;
    public ParticleSystem smokeParticles;

    Transform attackPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackPoint = Player.Instance.attackPoint;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
            case CrabState.idle:
                HandleIdle();
                break;
            case CrabState.preLaser:
                HandlePreLasering();
                break;
            case CrabState.lasering:
                HandleLasering();

                break;
            case CrabState.coolingDown:
                HandleCoolingDown();
                break;
        }
    }

    private void HandleIdle() {
        var distance = Vector3.Distance(transform.position, attackPoint.transform.position);

        if (distance < activationDistance)
            TransitionToPreLasering();

        //wander?
    }

    private void TransitionToPreLasering() {
        state = CrabState.preLaser;

        if (laserWindup != null) laserWindup.Play();
        laserWindup.pitch = minPitch;

        if (windupParticles != null) windupParticles.Play();

        preLaserTime = preLaserTimeAnimationCurve.Evaluate(GameManager.instance.difficulty);
        preLaserTimer = preLaserTime;
    }

    private void HandlePreLasering() {
        laserWindup.pitch += Time.deltaTime * pitchUpSpeed;

        lasering = laseringCube.DODynamicLookAt(attackPoint.position, preLaserLookSpeed).SetSpeedBased(true);

        if (preLaserTimer <= 0) {
            if (laserWindup != null) laserWindup.Stop();
            if (windupParticles != null) windupParticles.Stop();
            lasering.Kill();
            lasering = null;
            TransitionToLasering();
        }
        else
            preLaserTimer -= Time.deltaTime;
    }

    private void TransitionToLasering() {
        state = CrabState.lasering;

        if (laserFiring != null) laserFiring.Play();

        lineRenderer.enabled = true;

        laseringTime = laseringTimeAnimationCurve.Evaluate(GameManager.instance.difficulty);
        laseringTimer = laseringTime;
    }

    private void HandleLasering() {
        lineRenderer.SetPosition(0, laseringEnd.position);

        lasering = laseringCube.DODynamicLookAt(attackPoint.position, laserLookSpeed).SetSpeedBased(true);

        if (laseringTimer < laseringTime * 0.3)
            laserHotBitsRender.material = hotMaterial;
        else if (laseringTimer < laseringTime * 0.6)
            laserHotBitsRender.material = warmMaterial;
        else
            laserHotBitsRender.material = coolMaterial;

        Debug.DrawRay(laseringEnd.position, laseringEnd.forward * maxLaserDistance, Color.red);
        RaycastHit hit;
        if(Physics.Raycast(laseringEnd.position, laseringEnd.forward, out hit, maxLaserDistance, layerMask)) {
            lineRenderer.SetPosition(1, hit.point);

            laserFiring.transform.position = Vector3.Lerp(laseringEnd.position, hit.point, 0.5f);  //Place laser sound near center of laser

            Debug.DrawLine(transform.position, hit.point, Color.green);

            if (hit.collider.CompareTag("Player")) {
                hit.collider.SendMessage("OnHit", new DamageInstance(1, 1));
            } else if (hit.collider.CompareTag("Enemy")) {
                hit.collider.SendMessage("OnHit", new DamageInstance(1, 1));
            } else if (hit.collider.CompareTag("Mine")) {
                hit.collider.SendMessage("OnHit", new DamageInstance(1, 1));
            }
        }
        else {
            var distantLaserPoint = laseringEnd.position + laseringEnd.forward * maxLaserDistance;

            lineRenderer.SetPosition(1, distantLaserPoint);

            laserFiring.transform.position = Vector3.Lerp(laseringEnd.position, distantLaserPoint, 0.5f);  //Place laser sound near center of laser
        }

        if (laseringTimer <= 0) {
            if (laserFiring != null) laserFiring.Stop();
            lineRenderer.enabled = false;
            lasering.Kill();
            lasering = null;
            TransitionToCooldown();
        }
        else
            laseringTimer -= Time.deltaTime;
    }

    private void TransitionToCooldown() {
        state = CrabState.coolingDown;

        coolDownTime = coolDownTimeAnimationCurve.Evaluate(GameManager.instance.difficulty);
        coolDownTimer = coolDownTime;

        if (smokeParticles != null) smokeParticles.Play();
    }

    private void HandleCoolingDown() {
        if (coolDownTimer < coolDownTime * 0.3)
            laserHotBitsRender.material = coolMaterial; 
        else if (coolDownTimer < coolDownTime * 0.6)
            laserHotBitsRender.material = warmMaterial;
        else
            laserHotBitsRender.material = hotMaterial;

        if (coolDownTimer <= 0) {
            if (smokeParticles != null) smokeParticles.Stop();
            TransitionToPreLasering();
        }
        else
            coolDownTimer -= Time.deltaTime;
    }
}
