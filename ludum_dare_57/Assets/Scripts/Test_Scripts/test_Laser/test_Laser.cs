using UnityEngine;
using System.Collections.Generic;

public class test_Laser : MonoBehaviour
{
    public Transform start;
    public Transform end;

    public LineRenderer lineRenderer;

    public Material preWhiteMaterial;
    public Material whiteMaterial;

    [Range(0.0f, 2f)]
    public float timerBeforeShot;

    public float whiteThreshold = 1.5f;
    public float maxTime = 2f;

    public bool autoAdvance = false;

    public Gradient preWhiteGradient;
    public float emissionIntensity = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, end.position);

        lineRenderer.material = timerBeforeShot > whiteThreshold ? whiteMaterial : preWhiteMaterial;

        var lerped = Mathf.Lerp(0, whiteThreshold, timerBeforeShot);
       
        if(timerBeforeShot < whiteThreshold) {
            Color finalColor = preWhiteGradient.Evaluate(timerBeforeShot) * emissionIntensity;
            preWhiteMaterial.color = finalColor;
            lineRenderer.material.EnableKeyword("_EMISSION");
            lineRenderer.material.SetColor("_EmissionColor", finalColor);
        }

        if (autoAdvance) {
            timerBeforeShot += Time.deltaTime;
            if(timerBeforeShot > maxTime)
                timerBeforeShot = 0;
        }
    }
}
