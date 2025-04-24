using UnityEngine;
using System.Collections.Generic;

public class ThreatUI : MonoBehaviour
{
    [SerializeField] ThreatActor[] threats;
    [SerializeField] List<Transform> threatArrows = new List<Transform>();
    Player player;

    GameObject threatArrow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        threats = FindObjectsByType<ThreatActor>(FindObjectsSortMode.None);
        player = FindAnyObjectByType<Player>();

        foreach (var threat in threats) {
            threatArrows.Add(Instantiate(threatArrow).transform); //TODO pool this
        }
    }
}
