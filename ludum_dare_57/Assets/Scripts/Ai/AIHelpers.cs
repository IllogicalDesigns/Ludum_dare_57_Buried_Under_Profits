using UnityEngine;

public class AIHelpers : MonoBehaviour
{
    bool isDebug;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool CanThePlayerSeeUs(Transform ourTransform, Transform playerTransform, float activationDistance, float minActivationDistance, float dotRequirement, LayerMask layerMask) {
        Debug.DrawLine(ourTransform.position, playerTransform.position, Color.grey); // out of range

        if (!IsWithinRange(ourTransform, playerTransform, activationDistance, minActivationDistance)) return false;
        if (!IsInFront(ourTransform, playerTransform, dotRequirement)) return false;
        if (!IsLineOfSightClear(ourTransform, playerTransform, layerMask)) return false;

        Debug.DrawLine(ourTransform.position, playerTransform.position, Color.green); // Can see
        return true;
    }

    private static bool IsWithinRange(Transform ourTransform, Transform playerTransform, float activationDistance, float minActivationDistance) {
        var distance = Vector3.Distance(ourTransform.position, playerTransform.position);
        if (distance > activationDistance || distance < minActivationDistance) return false;

        Debug.DrawLine(ourTransform.position, playerTransform.position, Color.white); // In range
        return true;
    }

    private static bool IsInFront(Transform ourTransform, Transform playerTransform, float dotRequirement) {
        Vector3 heading = ourTransform.position - playerTransform.position;
        float dot = Vector3.Dot(heading.normalized, playerTransform.forward);

        if (dot < dotRequirement) return false;

        Debug.DrawLine(ourTransform.position, playerTransform.position, Color.yellow); // In front
        return true;
    }

    private static bool IsLineOfSightClear(Transform ourTransform, Transform playerTransform, LayerMask layerMask) {
        bool hasPlayersLineOfSight = !Physics.Linecast(ourTransform.position, playerTransform.position, layerMask);

        if (!hasPlayersLineOfSight) {
            Debug.DrawLine(ourTransform.position, playerTransform.position, Color.red); // Something in the way
        }

        return hasPlayersLineOfSight;
    }
}
