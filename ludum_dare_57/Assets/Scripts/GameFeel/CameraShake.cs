using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Transform cameraTRans;
    [SerializeField] float duration = 0.1f;
    [SerializeField] Vector3 positionPunch = Vector3.one;
    [SerializeField] Vector3 rotationPunch = Vector3.one * 15f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTRans = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHit(DamageInstance damageInstance) {
        var x = Random.Range(-positionPunch.x, positionPunch.x);
        var y = Random.Range(-positionPunch.y, positionPunch.y);
        var z = Random.Range(-positionPunch.z, positionPunch.z);

        cameraTRans.DOPunchPosition(new Vector3(x,y,z), duration);

        x = Random.Range(-rotationPunch.x, rotationPunch.x);
        y = Random.Range(-rotationPunch.y, rotationPunch.y);
        z = Random.Range(-rotationPunch.z, rotationPunch.z);
        cameraTRans.DOPunchRotation(new Vector3(x, y, z), duration);
    }
}
