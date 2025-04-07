using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Transform damageLocation;
    public Transform player;
    public Transform damageIndicatorPivot;
    [Space]
    public Color farColor = Color.gray;
    public Color nearColor = Color.red;
    public Color damageColor = Color.white;
    [Space]
    public float damgeThresh = 3f;
    public float nearThresh = 7f;
    [Space]
    public Image damgeImage;
    [Space]
    public float angleRequirement = 30f;
    public float dotRequirement = 0.8f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Player>().transform;
    }

    // Update is called once per frame
    void Update() {
        if (damageLocation == null) {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (damageLocation.position - player.position).normalized;
        float angle = (Vector3.SignedAngle(direction, player.forward, Vector3.up));
        damageIndicatorPivot.transform.localEulerAngles = new Vector3(0, 0, angle);

        var color = ChangeColorsBasedOnDistance();
        damgeImage.color = color;

        Vector3 directionToDamage = (damageLocation.position - player.position).normalized; // Vector pointing from player to damage location
        float dotProduct = Vector3.Dot(player.forward, directionToDamage); // Dot product between player's forward direction and the direction to damage location

        damgeImage.gameObject.SetActive(!(dotProduct > dotRequirement));

        //float angleHere = Vector3.Angle(player.forward, directionToDamage);

        //damgeImage.gameObject.SetActive(!(angleHere < 30f));
        //if (angleHere < 30f) // Tighten by reducing this angle
        //{
        //    Debug.Log("The damage location is within a tight frontal cone.");
        //}
        //else {
        //    Debug.Log("The damage location is outside the frontal cone.");
        //}
    }

    private Color ChangeColorsBasedOnDistance() {
        float distance = Vector3.Distance(damageLocation.position, player.position);
        if (distance < damgeThresh)
            return damageColor;
        else if (distance < nearThresh)
            return nearColor;
        else
            return farColor;
    }
}
