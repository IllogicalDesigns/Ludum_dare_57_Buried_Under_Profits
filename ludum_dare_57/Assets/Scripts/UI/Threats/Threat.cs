using UnityEngine;

public class Threat : MonoBehaviour
{
    public const string becomeThreatString = "BecomeThreat";
    public const string unBecomeThreat = "UnBecomeThreat";

    GameObject myThreatIndicator;
    [SerializeField] GameObject threatIndicatorPrefab;

    //TODO add threat level which aguments the color of the threat
    public void BecomeThreat() {
        if (myThreatIndicator != null) return;

        var panelTransform = GameObject.Find("Threat Panel")?.transform;
        myThreatIndicator = Instantiate(threatIndicatorPrefab, panelTransform); //TODO pool this
        DamageIndicator threatActor = myThreatIndicator.GetComponent<DamageIndicator>();
        threatActor.damageLocation = this.transform;
    }

    public void UnBecomeThreat() {
        if(myThreatIndicator != null) {
            Destroy(myThreatIndicator);
        }
    }
}
