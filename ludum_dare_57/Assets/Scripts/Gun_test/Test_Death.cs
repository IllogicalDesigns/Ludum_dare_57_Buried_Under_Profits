using UnityEngine;

public class Test_Death : MonoBehaviour, IDeathReaction {
    //public GameObject deathPrefab;
    public float force = 5f;
    public void Die(DamageInstance damageInstance) {
        if (damageInstance.hit.collider != null) {
            Vector3 pos = transform.position;
            Quaternion rot = Quaternion.identity;
            //var obj = Instantiate(deathPrefab, pos, rot) as GameObject;

            var rigid = gameObject.GetComponent<Rigidbody>().GetComponent<Rigidbody>();
            rigid.isKinematic = false;
            rigid.AddForceAtPosition(-damageInstance.hit.normal * force, damageInstance.hit.point, ForceMode.Impulse);

            //gameObject.SetActive(false);
        }
    }
}