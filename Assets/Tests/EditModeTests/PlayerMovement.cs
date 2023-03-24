using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{   
    public enum PlayerState{
        move,
        attack,
        interact
    }
    public float playerSpeed;
    private PlayerState currentState;
    private Rigidbody2D playerRidgedbody;
    private Animator playerAnimator;
    private Vector3 playerMovementChange;

    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerState.move;
        playerRidgedbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMovementChange = Vector3.zero;
        playerMovementChange.x = Input.GetAxisRaw("Horizontal");
        playerMovementChange.y = Input.GetAxisRaw("Vertical");
        Debug.Log(playerMovementChange);
        MoveCharacter();
        /*
        if(Input.GetButtonDown("Interact") && currentState != PlayerState.interact){
            StartCoroutine(InteractCoRoutine());//still need to write the interactCoRoutine method
        }
        else if(currentState == PlayerState.move){
            UpdateAnimationAndMove();
        }
        */
    }

        /*private IEnumerator InteractCoRoutine(){
            playerAnimator.SetBool("interacting", true);
            currentState = PlayerState.interact;
            yield return null;
            playerAnimator.SetBool("interacting", false);
            yield return new WaitForSeconds(0.3f);
            currentState = PlayerState.move;
        }

        void UpdateAnimationAndMove(){
            if(playerMovementChange != Vector3.zero){
                MoveCharacter();
                playerAnimator.SetFloat("setX", playerMovementChange.x);
                playerAnimator.SetFloat("setY", playerMovementChange.y);
                playerAnimator.SetBool("moving", true);
            }
            else{
                playerAnimator.SetBool("moving", false);
            }
        }
        */

    void MoveCharacter(){
        playerRidgedbody.MovePosition(transform.position + playerMovementChange *
            playerSpeed * Time.deltaTime);
    }
}

