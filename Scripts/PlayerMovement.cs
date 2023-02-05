using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    public float playerSpeed;
    private Rigidbody2D playerRidgedbody;
    private Vector3 playerMovementChange;

    // Start is called before the first frame update
    void Start()
    {
        playerRidgedbody = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMovementChange = Vector3.zero;
        playerMovementChange.x = Input.GetAxisRaw("Horizontal");
        playerMovementChange.y = Input.GetAxisRaw("Vertical");
        Debug.Log(playerMovementChange);

        if(playerMovementChange != Vector3.zero){
            MoveCharacter();
        }
            
    }

    void MoveCharacter(){
        playerRidgedbody.MovePosition(transform.position + playerMovementChange *
            playerSpeed * Time.deltaTime);


    }
}
