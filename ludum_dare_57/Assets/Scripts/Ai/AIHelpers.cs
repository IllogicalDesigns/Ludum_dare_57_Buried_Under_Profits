using UnityEngine;

public class AIHelpers : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool canThePlayerSeeUs(Transform ourTransform, Transform playerTransform, float activationDistance, float minActivationDistance, float dotRequirement, LayerMask layerMask) {
        Debug.DrawLine(ourTransform.position, playerTransform.position, Color.gray);

        var distance = Vector3.Distance(ourTransform.position, playerTransform.position);
        if (distance > activationDistance) return false; //Too far
        if (distance < minActivationDistance) return false; //Too close
        Debug.DrawLine(ourTransform.position, playerTransform.position, Color.white); //in range

        Vector3 heading = playerTransform.position - ourTransform.position;
        float dot = Vector3.Dot(heading.normalized, playerTransform.forward);
        if (dot > dotRequirement) return false; //Not infront
        Debug.DrawLine(ourTransform.position, playerTransform.position, Color.yellow); //In front

        bool hasPlayersLineOfSight = !Physics.Linecast(ourTransform.position, playerTransform.position, layerMask);
        if (!hasPlayersLineOfSight) {
            Debug.DrawLine(ourTransform.position, playerTransform.position, Color.red);
            return false;  //Something in the way
        }

        Debug.DrawLine(ourTransform.position, playerTransform.position, Color.green); //Can see
        return true;
    }
}
