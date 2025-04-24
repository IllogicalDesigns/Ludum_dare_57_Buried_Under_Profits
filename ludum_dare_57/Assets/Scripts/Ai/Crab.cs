using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public float preLaserAgentSpeed = 5f;

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
    public float laserAgentSpeed = 2.5f;

    [Space]
    public Renderer laserHotBitsRender;
    Material hotBitMaterial;
    public Gradient heatGradient;
    public float boostAmount = 2f;

    [Space]
    float coolDownTime = 3f;
    public AnimationCurve coolDownTimeAnimationCurve = new AnimationCurve(new Keyframe(0f, 6f), new Keyframe(2, 2f));
    float coolDownTimer;
    public ParticleSystem smokeParticles;

    Transform attackPoint;

    [Space]
    public NavMeshAgent agent;
    public float agentDistance = 10f;

    [Space]
    Dictionary<Collider, float> hitObjects = new Dictionary<Collider, float>();
    float timeBetweenTicks = 1f;
    int damage = 2;
    int airDamage = 1;
    Threat threat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackPoint = Player.Instance.transform;
        hotBitMaterial = laserHotBitsRender.material;
        hotBitMaterial.EnableKeyword("_EMISSION");

        threat = GetComponent<Threat>();

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

        if (distance < activationDistance) {
            threat.BecomeThreat();
            TransitionToPreLasering();
        }
            
        //wander?
    }

    private void TransitionToPreLasering() {
        state = CrabState.preLaser;

        if (laserWindup != null) laserWindup.Play();
        laserWindup.pitch = minPitch;

        if (windupParticles != null) windupParticles.Play();

        agent.speed = preLaserAgentSpeed;

        preLaserTime = preLaserTimeAnimationCurve.Evaluate(GameManager.instance.difficulty);
        preLaserTimer = preLaserTime;
    }

    private void HandlePreLasering() {
        laserWindup.pitch += Time.deltaTime * pitchUpSpeed;

        lasering = laseringCube.DODynamicLookAt(attackPoint.position, preLaserLookSpeed).SetSpeedBased(true);

        MoveIntoActRange();

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

        hitObjects.Clear();

        agent.speed = laserAgentSpeed;
    }

    private void HandleLasering() {
        lineRenderer.SetPosition(0, laseringEnd.position);

        lasering = laseringCube.DODynamicLookAt(attackPoint.position, laserLookSpeed).SetSpeedBased(true);

        float t = Mathf.Clamp01(laseringTimer / laseringTime);

        Color emissionColor = heatGradient.Evaluate(t) * boostAmount; // Boost intensity if needed
        hotBitMaterial.SetColor("_EmissionColor", emissionColor);

        MoveIntoActRange();

        Debug.DrawRay(laseringEnd.position, laseringEnd.forward * maxLaserDistance, Color.red);
        RaycastHit hit;
        if(Physics.Raycast(laseringEnd.position, laseringEnd.forward, out hit, maxLaserDistance, layerMask)) {
            OnRaycastHit(hit);
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

    private void OnRaycastHit(RaycastHit hit) {
        lineRenderer.SetPosition(1, hit.point);

        laserFiring.transform.position = Vector3.Lerp(laseringEnd.position, hit.point, 0.5f);  //Place laser sound near center of laser

        Debug.DrawLine(transform.position, hit.point, Color.green);

        if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Mine")) {
            const float nudge = 0.1f;

            if (!hitObjects.ContainsKey(hit.collider)) {
                hitObjects.Add(hit.collider, Time.time-nudge);
            } 
            
            if(hitObjects[hit.collider] < Time.time) {
                hit.collider.SendMessage("OnHit", new DamageInstance(damage, airDamage));
                hitObjects[hit.collider] = Time.time + timeBetweenTicks;
            }
        }
    }

    private void TransitionToCooldown() {
        state = CrabState.coolingDown;

        coolDownTime = coolDownTimeAnimationCurve.Evaluate(GameManager.instance.difficulty);
        coolDownTimer = coolDownTime;

        if (smokeParticles != null) smokeParticles.Play();
    }

    private void HandleCoolingDown() {
        float t = Mathf.Clamp01(coolDownTimer / coolDownTime);
        float invertedT = 1.0f - t;

        Color emissionColor = heatGradient.Evaluate(invertedT) * boostAmount; // Boost intensity if needed
        hotBitMaterial.SetColor("_EmissionColor", emissionColor);

        lasering = laseringCube.DODynamicLookAt(attackPoint.position, preLaserLookSpeed).SetSpeedBased(true);

        if (coolDownTimer <= 0) {
            if (smokeParticles != null) smokeParticles.Stop();
            lasering.Kill();
            lasering = null;
            TransitionToPreLasering();
        }
        else
            coolDownTimer -= Time.deltaTime;
    }

    private void MoveIntoActRange() {
        var distance = Vector3.Distance(transform.position, attackPoint.transform.position);
        if (distance > agentDistance) {
            agent.SetDestination(attackPoint.position);
            agent.isStopped = false;
        }
        else if (!agent.isStopped)
            agent.isStopped = true;
    }
}
