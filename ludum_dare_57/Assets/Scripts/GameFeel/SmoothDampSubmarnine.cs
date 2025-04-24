using UnityEngine;

public class SmoothDampSubmarnine : MonoBehaviour
{
    [SerializeField] Transform playerTrans;
    [SerializeField] Transform submarineTrans;
    [SerializeField] Vector3 offset = new Vector3(0.258f, -0.033f, 0.84f);
    [SerializeField] float rotationSpeed = 2f; // Adjust this value for desired 

    // Update is called once per frame
    void LateUpdate()
    {
        submarineTrans.position = playerTrans.position + offset;

        submarineTrans.rotation = Quaternion.Slerp(submarineTrans.rotation, playerTrans.rotation, Time.deltaTime * rotationSpeed);

    }
}
