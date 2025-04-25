using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ForceZone : MonoBehaviour
{
    public Vector3 force = Vector3.up * 10f;

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true; 
    }

    private void OnTriggerStay(Collider other){
        if(GameManager.instance.currentGameState != GameManager.GameState.playing) return;

        if(other.CompareTag("Player")){
            var characterController = other.GetComponent<CharacterController>();
            characterController.Move(force * Time.deltaTime);
        }
        else if(other.CompareTag("Enemy")){
            //Move the enemy 
        }
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, force);
    }
}
