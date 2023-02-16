using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerControler : MonoBehaviour
{

    public float maxSpeed = 5f; //This is the maximum speed the character can go
    public float acceleration = 5f; //how fast we accelerate to max speed. To find out how long it takes you to accelerate, simply divide maxSpeed by acceleration. That is how long is seconds it takes
    public Rigidbody2D rb; //Rigidbody of the player character
    public Animator animator; //animator controller for the player

    [SerializeField] private GameObject tilemap;

    private Vector2 movement; //the direction the player is moving in
    private float currentSpeed = 0;
    int direction = 2;        //Keeps track of what direction we are facing: 0 = Up, 1 = Right, 2 = Down, 3 = Left


    // Start is called before the first frame update
    // Could whoever added this code please comment it? I think it's for spawning in the character, but honestly it's hard to tell
    IEnumerator Start()
    {

        tilemap = GameObject.Find("Tilemap");
        Tilemap tmap = tilemap.GetComponent<Tilemap>();
        LevelGeneration.Map code = tilemap.GetComponent<LevelGeneration.Map>();

        yield return new WaitUntil(() => code.isDone);
    
        for(int x = 0; x < 10 * 10 + 2; x++) {
            for(int y = 0; y < 10 * 8 + 2; y++) {
                var celPos = new Vector3Int(x, y, 0);
                var sprite = tmap.GetSprite(celPos);
                if(sprite) {
                    if(sprite.name == "Start") {
                        rb.position = new Vector2(celPos.x, celPos.y);
                    }   
                }
            }
        }    
    }


    IEnumerator waiter()
    {
        yield return new WaitForSeconds(2);
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal"); //Get the movement that the user is currently inputting
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize(); //Normalize the result so the vector has a magnitude of 1, regardless of the exact values. This allows for easy tracking of direction
    }

    private void FixedUpdate() //fixed updated doesn't rely on framerate, and is where you should actually apply movement systems
    {
        //if the player is inputting anything, then start accelerating in that direction. Not using the actuall acceleration system as that can be prone to velocity issues
        if (currentSpeed <= maxSpeed && movement.sqrMagnitude != 0){
            currentSpeed = currentSpeed + acceleration * Time.fixedDeltaTime;
            if(currentSpeed > maxSpeed) {
                currentSpeed = maxSpeed;
            }
            //if the movement in the X direction is greater or equal to the movement in the y direction, the point the character along the x direction
            if(Mathf.Abs(movement.x) >= Mathf.Abs(movement.y)){
                //Mathf.Sign will return either -1 or 1, giving us either 1 or 3 for the direction, exactly what we want
                //We subtract instead of add because of how the coordinate field and our defined directions interact
                direction = 2 - (int)Mathf.Sign(movement.x);
            }else{
                //Same as above, only with an offset of 1 instead of 2
                direction = 1 - (int)Mathf.Sign(movement.y);
            }
        }
        else if (currentSpeed > 0){
            currentSpeed = 0;
        }
        //move the player by there current speed, with Time.fixedDeltaTime making sure movement is consistent
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
        animator.SetFloat("Speed",currentSpeed);
        animator.SetInteger("Direction",direction);
    }
}
 