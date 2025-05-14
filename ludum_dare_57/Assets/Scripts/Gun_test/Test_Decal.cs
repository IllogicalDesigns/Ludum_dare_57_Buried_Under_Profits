using UnityEngine;

public class Test_Decal : MonoBehaviour, IHitReaction {
    public GameObject decalPrefab;

    public void React(DamageInstance damageInstance) {
        if (decalPrefab && damageInstance.hit.collider != null) {
            Vector3 pos = damageInstance.hit.point;
            Quaternion rot = Quaternion.LookRotation(-damageInstance.hit.normal);
            var obj = Instantiate(decalPrefab, pos, rot) as GameObject;
            obj.transform.SetParent(damageInstance.hit.transform);
        }
    }
}
