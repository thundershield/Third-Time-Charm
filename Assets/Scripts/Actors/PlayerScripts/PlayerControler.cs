using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using LevelGeneration;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class PlayerControler : MonoBehaviour
{
    private const float InvincibilityTime = 0.5f;
    private const float AttackTime = 0.30f;
    private const float AttackHitboxSize = 1f;
    
    public int health = 100;
    public int armor = 0;
    public int speed = 0;
    public int damage = 10;
    private float invincibilityTimer;
    private float attackTimer;

    public float maxSpeed = 5f; //This is the maximum speed the character can go
    public float acceleration = 5f; //how fast we accelerate to max speed. To find out how long it takes you to accelerate, simply divide maxSpeed by acceleration. That is how long is seconds it takes
    public Rigidbody2D rb; //Rigidbody of the player character
    public Animator animator; //animator controller for the player

    public inventory playerInventory; // reference to the inventory for when player colides with item

    private Vector2 movement; //the direction the player is moving in
    private float currentSpeed = 0;
    public int directionX = 0;
    public int directionY = -1;

    void OnTriggerEnter2D(Collider2D other)
    {   
        // if(other.gameObject.tag == "itemObject" && Input.GetKeyDown(KeyCode.F)) {
        //     if(playerInventory.addItem(null)) { Destroy(other.gameObject); }
        // }
    }

    private Map map;
    private Vector2 endPosition;

    public void updateStats(statBlock stats) {
        armor = stats.armor;
        speed = stats.speed;
        maxSpeed = 5f + stats.speed/5;
    }

    private void Awake()
    {
        var playerInventoryObject = GameObject.FindGameObjectWithTag("Inventory");
        playerInventory = playerInventoryObject.GetComponent<inventory>();
        playerInventory.mainInventory.updateStatWindow(this);
    }

    private void Start()
    {
        map = GameObject.Find("TilemapGrid/Tilemap").GetComponent<Map>();
        if (map is null) throw new NullReferenceException("Couldn't find the map!");
        endPosition = map.GetLastLoadData().EndPosition;
    }

    private void RestartLevel()
    {
        // Regenerating the map also destroys all enemies in the map.
        // So restarting the level just requires regenerating the map.
        map.Generate();
    }

    private void CheckReachedEnd()
    {
        var position2d = new Vector2(transform.position.x, transform.position.y);
        if (Vector2.Distance(position2d, endPosition) < 1.0f)
        {
            // The player has reached the end of the level, create a new map.
            RestartLevel();
        }
    }

    private void showPopup(bool openness, List<string> items = null) {
        GameObject popup = GameObject.Find("textPopupBackground");
        popup.GetComponent<Image>().enabled = openness;
        popup.transform.Find("textPopup").gameObject.SetActive(openness);
        if(items != null) {
            string text = "Press \"F\" to pick up item\n" + string.Join("\n",items);
            popup.transform.Find("textPopup").gameObject.GetComponent<TextMeshProUGUI>().text = text;
        }
    }

    void Update()
    {
        invincibilityTimer -= Time.deltaTime;
        GameObject.Find("HealthBar").GetComponent<Slider>().value = health/100f;
        GameObject.Find("HealthNumber").GetComponent<TextMeshProUGUI>().text = "Health: " + health.ToString();
        
        Collider2D coll = GetComponent<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        filter.SetLayerMask(LayerMask.GetMask("Item"));
        List<Collider2D> results = new List<Collider2D>();
        int num = Physics2D.OverlapCollider(coll,filter,results);

        List<string> items = new List<string>();
        if(num == 0) { showPopup(false); }
        else {
            foreach( Collider2D item in results) {
                if (item.gameObject.tag == "itemObject") {
                    items.Add(item.GetComponent<itemObject>().name);
                }
            }
        }
        for(int i = 0; i< num; i++) {
            if (results[i].gameObject.tag == "itemObject") {
                if(!playerInventory.isFull()) { showPopup(true, items); }
                if(Input.GetKeyDown(KeyCode.F)) {
                    if(playerInventory.addItem(results[i].GetComponent<itemObject>().id)) { Destroy(results[i].gameObject); } 
                }
            }
        }
        
        if (attackTimer <= 0f && Input.GetButton("Fire1"))
        {
            animator.Play("Attack");
            attackTimer = AttackTime;
            
            // Attack anything in front of the player:
            if(playerInventory.mainInventory.currentItem != null) { 
                playerInventory.mainInventory.currentUsableItem.use(); 
            }
        }
        
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
        
        // Runs only if the player is not attacking:

        movement.x = Input.GetAxisRaw("Horizontal"); //Get the movement that the user is currently inputting
        movement.y = Input.GetAxisRaw("Vertical");
        if (movement.x != 0f || movement.y != 0f)
        {
            directionX = movement.x switch
            {
                < 0 => -1,
                > 0 => 1,
                _ => 0
            };
            directionY = movement.y switch
            {
                < 0 => -1,
                > 0 => 1,
                _ => 0
            };
        }
        movement.Normalize(); //Normalize the result so the vector has a magnitude of 1, regardless of the exact values. This allows for easy tracking of direction
    }

    private void FixedUpdate() //fixed updated doesn't rely on framerate, and is where you should actually apply movement systems
    {
        CheckReachedEnd();
        
        //if the player is inputting anything, then start accelerating in that direction. Not using the actuall acceleration system as that can be prone to velocity issues
        if (currentSpeed <= maxSpeed && movement.sqrMagnitude != 0){
            currentSpeed = currentSpeed + acceleration * Time.fixedDeltaTime;
            if(currentSpeed > maxSpeed) {
                currentSpeed = maxSpeed;
            }
        }
        else if (currentSpeed > 0){
            currentSpeed = 0;
        }
        //move the player by there current speed, with Time.fixedDeltaTime making sure movement is consistent
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
        animator.SetFloat("Speed",currentSpeed);
        
        animator.SetFloat("DirectionX",directionX);
        animator.SetFloat("DirectionY",directionY);
    }

    public void TakeDamage(int damage)
    {
        if (invincibilityTimer > 0f) return;

        invincibilityTimer = InvincibilityTime;
        health -= damage;
        playerInventory.mainInventory.updateStatWindow();

        if (health > 0) return;

        RestartLevel();
    }
}
 