using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider2D))]

public class player_1 : MonoBehaviour
{
    private Vector3 moveDelta;
    private BoxCollider2D boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = getComponent<BoxCollider2D>();   
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        
        float x = Input.GetAxisRaw("Horizontal");//gets input for x and y axis movement of sprite
        float y = Input.GetAxisRaw("Vertical");

        moveDelta = new Vector3(x, y, 0); //resets the moveDelta at the beginning of the update.

       // Debug.Log(x);//checks to see if movement controls are working in the conlsole
       // Debug.Log(y);

       if(moveDelta.x > 1){//changes sprite direcion when movement buttons are pressed.
            transform.localScale = Vector3(1, 1, 1);
       }
       else if(moveDelta.x < 1){
            transform.localScale = new Vector3(-1, 1, 1);
       }

       transform.Translate(moveDelta * Time.deltaTime);//moves sprite
       

       
    }
}
