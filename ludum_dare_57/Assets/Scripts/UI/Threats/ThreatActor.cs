using DG.Tweening;
using UnityEngine;

public class ThreatActor : MonoBehaviour
{
    public Transform threat;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform arrowTransform;
    [Space]
    [SerializeField] RectTransform arrowRectTransform;
    [SerializeField] GameObject arrowGameObject;
    [SerializeField] LayerMask layerMask = ~0;

    [SerializeField] Vector2 minSize = new Vector2(10, 10);
    [SerializeField] Vector2 maxSize = new Vector2(50, 50);
    [SerializeField] float minDistance = 30f;
    [SerializeField] float maxDistance = 5f;

    [SerializeField] float pulseTime = 0.25f;
    [SerializeField] float pulseScale = 1.4f;
    [SerializeField] float pulseTrigger = 10f;

    private Vector2 targetSize;

    Tween tween;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = FindAnyObjectByType<Player>().transform;

        arrowGameObject = arrowTransform.gameObject;
        arrowRectTransform = arrowTransform.GetComponent<RectTransform>();

        targetSize = minSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (threat == null) { 
            Destroy(gameObject);
            return;
        }

        // Convert world position to viewport position
        var threatPosition = threat.position;
        Vector3 screenPos = Camera.main.WorldToViewportPoint(threatPosition);

        // Get RectTransform of the arrow
        RectTransform arrowRectTransform = arrowGameObject.GetComponent<RectTransform>();

        if (arrowRectTransform == null) return;

        // Check if threat is behind camera
        if (screenPos.z <= 0) {
            // Place arrow at bottom of screen for threats behind player
            Vector2 bottomScreenPoint = new Vector2(Screen.width / 2f, 50f); // Adjust "50f" for padding from bottom
            arrowRectTransform.position = bottomScreenPoint;

            arrowGameObject.SetActive(true);
            return;
        }

        var distance = Vector3.Distance(threatPosition, playerTransform.position);
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        arrowRectTransform.sizeDelta = Vector2.Lerp(minSize, maxSize, t);
        if (tween == null && distance < pulseTrigger)
            tween = arrowRectTransform.transform.DOScale(Vector3.one * pulseScale, pulseTime).SetLoops(-1, LoopType.Yoyo);
        if (tween != null && distance > pulseTrigger) {
            tween.Kill(true);
            arrowRectTransform.transform.localScale = Vector3.one;
            tween = null;
        }

            // Check if threat is off-screen
            bool isOffScreen = screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1;

        //|| Physics.Linecast(playerTransform.position, arrowTransform.position, layerMask)
        if (isOffScreen ) {
            // Clamp position to viewport edges
            screenPos.x = Mathf.Clamp(screenPos.x, 0.05f, 0.95f);
            screenPos.y = Mathf.Clamp(screenPos.y, 0.05f, 0.95f);

            // Convert viewport position to screen space
            Vector2 screenPoint = Camera.main.ViewportToScreenPoint(screenPos);

            // Set arrow position on canvas
            arrowRectTransform.position = screenPoint;

            // Activate the arrow
            arrowGameObject.SetActive(true);
        }
        else {
            // Hide arrow if threat is on-screen
            arrowGameObject.SetActive(false);
        }
    }
}
