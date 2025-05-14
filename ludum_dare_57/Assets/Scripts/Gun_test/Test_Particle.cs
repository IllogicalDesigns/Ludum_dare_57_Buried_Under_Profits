using UnityEngine;

public class Test_Particle : MonoBehaviour, IHitReaction 
{
    public GameObject particlePrefab;

    public void React(DamageInstance damageInstance) {
        if (particlePrefab && damageInstance.hit.collider != null) {
            Vector3 pos = damageInstance.hit.point;
            Quaternion rot = Quaternion.LookRotation(damageInstance.hit.normal);
            var obj = Instantiate(particlePrefab, pos, rot) as GameObject;

            //var decal = obj.transform.Find("Decal Projector");
            //if (decal != null) {
            //    decal.transform.position = pos;
            //    decal.transform.rotation = Quaternion.LookRotation(-damageInstance.hit.normal);
            //    decal.transform.SetParent(damageInstance.hit.transform);
            //}
        }
    }
}
