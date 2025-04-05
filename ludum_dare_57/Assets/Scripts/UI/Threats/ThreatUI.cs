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
            threatArrows.Add(Instantiate(threatArrow).transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.transform.position;

        for (int i = 0; i < threats.Length; i++) {
            //ThreatActor threat = threats[i];
            //if (threat == null) continue;

            //Vector3 threatPosition = threat.transform.position;


            //Vector3 screenPos = Camera.main.WorldToViewportPoint(threatPosition);
            //screenPos.x = Mathf.Clamp(screenPos.x, 0.05f, 0.95f);
            //screenPos.y = Mathf.Clamp(screenPos.y, 0.05f, 0.95f);

            //Vector3 direction = (threatPosition - playerPosition).normalized;
            //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //threatArrows[i].rotation = Quaternion.Euler(0, 0, angle);

            //Vector2 screenPoint = Camera.main.ViewportToScreenPoint(screenPos);
            //arrowRectTransform.position = screenPoint;

        }
    }
}
