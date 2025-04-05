using UnityEngine;

public class Threat : MonoBehaviour
{
    public const string becomeThreatString = "BecomeThreat";
    public const string unBecomeThreat = "UnBecomeThreat";

    GameObject myThreatIndicator;
    [SerializeField] GameObject threatIndicatorPrefab;

    public void BecomeThreat() {
        var panelTransform = GameObject.Find("Threat Panel")?.transform;
        myThreatIndicator = Instantiate(threatIndicatorPrefab, panelTransform);
        ThreatActor threatActor = myThreatIndicator.GetComponent<ThreatActor>();
        threatActor.threat = transform;
    }

    public void UnBecomeThreat() {
        if(myThreatIndicator != null) {
            Destroy(myThreatIndicator);
        }
    }
}
