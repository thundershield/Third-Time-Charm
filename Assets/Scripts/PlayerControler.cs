using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerControler : MonoBehaviour
{

    private Rigidbody2D rb;

    private GameObject tilemap;

    private float horizontal;
    private float vertical;


    public BoundsInt area;


    // Start is called before the first frame update
    IEnumerator Start()
    {

        rb = GetComponent<Rigidbody2D>();

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

    // Update is called once per frame
    void Update()
    {
        //Press the Up arrow key to move the RigidBody upwards
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(xMove * 10, yMove * 10);
    }
}
 