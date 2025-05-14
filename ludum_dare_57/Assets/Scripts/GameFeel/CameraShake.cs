using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Transform cameraTRans;
    [Space]
    [SerializeField] float duration = 0.1f;
    [SerializeField] Vector3 positionPunch = Vector3.one;
    [SerializeField] Vector3 rotationPunch = Vector3.one * 15f;
    [Space]
    [SerializeField] float collisionDuration = 0.5f;
    [SerializeField] Vector3 collisionPositionPunch = Vector3.one * 0.1f;
    [SerializeField] Vector3 collisionRotationPunch = Vector3.one * 0.1f;
    [Space]
    [SerializeField] float laserDuration = 0.5f;
    [SerializeField] Vector3 laserPositionPunch = Vector3.one * 0.1f;
    [SerializeField] Vector3 laserRotationPunch = Vector3.one * 0.1f;

    private void Start() {
        if(TryGetComponent<Test_gun>(out Test_gun gun)) {
            gun.OnFireEvent += PunchScreen;
        }
    }

    private void OnDestroy() {
        if (TryGetComponent<Test_gun>(out Test_gun gun)) {
            gun.OnFireEvent -= PunchScreen;
        }
    }

    public void PunchScreen() {
        PunchScreen(duration, positionPunch, rotationPunch);
    }

    public void PunchScreen(float _duration, Vector3 _positionPunch, Vector3 _rotationPunch) {
        cameraTRans.DOKill(true);

        var x = Random.Range(-_positionPunch.x, _positionPunch.x);
        var y = Random.Range(-_positionPunch.y, _positionPunch.y);
        var z = Random.Range(-_positionPunch.z, _positionPunch.z);

        cameraTRans.DOPunchPosition(new Vector3(x, y, z), _duration);

        x = Random.Range(-_rotationPunch.x, _rotationPunch.x);
        y = Random.Range(-_rotationPunch.y, _rotationPunch.y);
        z = Random.Range(-_rotationPunch.z, _rotationPunch.z);
        cameraTRans.DOPunchRotation(new Vector3(x, y, z), _duration);
    }

    public void OnHit(DamageInstance damageInstance) {
        switch (damageInstance.damageType) {
            case DamageInstance.DamageType.collision:
                PunchScreen(collisionDuration, collisionPositionPunch, collisionRotationPunch);
                break;
            case DamageInstance.DamageType.laser:
                PunchScreen(laserDuration, laserPositionPunch, laserRotationPunch);
                break;
            default:
                PunchScreen(duration, positionPunch, rotationPunch);
                break;
        }
    }
}
