using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform playerTarget;
    public float cameraSmoothing;
    void Start()
    {
        
    }


    private void LateUpdate()
    {
        if(transform.position != playerTarget.position){
            Vector3 cameraTargetPosition = new Vector3(playerTarget.position.x, playerTarget.position.y,
            transform.position.z);
            transform.position = Vector3.Lerp(transform.position, cameraTargetPosition,
            cameraSmoothing);
        }
    }

}
