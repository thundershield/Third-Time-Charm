using System;
using UnityEngine;
using Random = System.Random;
using Pathfinding;

namespace Enemies
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour
    {
        public Transform target; //will usually hold the Transform of the player
        //[SerializeField] private float frameLength = 0.2f;
        //[SerializeField] private float directionChangeTime = 1.5f;
        [SerializeField] private float maxSpeed = 20.0f;
        [SerializeField] private int maxHealth = 50;
        private int curHealth;

        //private static readonly Random Random = new();
        
        //[SerializeField] private Sprite[] walkDownSprites;
        //[SerializeField] private Sprite[] walkLeftSprites;
        //[SerializeField] private Sprite[] walkRightSprites;
        //[SerializeField] private Sprite[] walkUpSprites;
        
        //private SpriteRenderer _spriteRenderer;
        //private float _animationTimer;
        //private float directionChangeTimer;
        private Direction direction;
        private Vector2 directionVec;
        private float targetDistance = 1000; //how far the target is, following the path
        private Rigidbody2D rb;
        private Seeker seeker;
        private Path path;
        private float wayPointDistanceThreshold = 0.5f; //How close we need to get to the target waypoint before getting a new target
        int targetWaypoint = 0;
        
        private void Start()
        {
            target = GameObject.Find("Player(Clone)").transform; //this is a temporary measure for testing purposes. Replace with proper system
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            seeker = GetComponent<Seeker>();
            InvokeRepeating("FindPath",0f,.25f);
            curHealth = maxHealth;
            seeker.StartPath(rb.position, target.position, PathFound);
        }
        void FindPath(){
            if(seeker.IsDone()){//check that we're not already finding a path
                seeker.StartPath(rb.position, target.position, PathFound);
            }
        }
        void PathFound(Path newPath)
        {
            if(!newPath.error){
                path = newPath;
                targetDistance = path.GetTotalLength();
            }
        }

        private void FixedUpdate()
        {
            if(path != null){
                if (targetWaypoint >= path.vectorPath.Count) {
                    return;
                }
                if(targetDistance < 20){
                    directionVec = ((Vector2)path.vectorPath[targetWaypoint] - rb.position).normalized;//get the normalized vector pointing towards the current waypoint
                    rb.MovePosition(rb.position + directionVec * maxSpeed * Time.fixedDeltaTime);
                    //Increments the target waypoint if we are within distance
                    if(((rb.position - (Vector2)path.vectorPath[targetWaypoint]).magnitude)<wayPointDistanceThreshold) targetWaypoint++;
                }
            }
        }
        private void Update()
        {
            //_animationTimer += Time.deltaTime;
            //directionChangeTimer += Time.deltaTime;
//
            //if (directionChangeTimer > directionChangeTime)
            //{
            //    directionChangeTimer = 0f;
            //    ChangeDirection();
            //}
//
            //var sprites = GetSpritesForDirection(direction);
            //var frame = Mathf.FloorToInt(_animationTimer / frameLength) % sprites.Length;
            //_spriteRenderer.sprite = sprites[frame];
        }

        //private void ChangeDirection()
        //{
        //    direction = (Direction)Random.Next(4);
        //    directionVec = direction switch
        //    {
        //        Direction.Up => Vector2.up,
        //        Direction.Down => Vector2.down,
        //        Direction.Left => Vector2.left,
        //        Direction.Right => Vector2.right,
        //        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        //    };
        //}
//
        //private Sprite[] GetSpritesForDirection(Direction direction)
        //{
        //    return direction switch
        //    {
        //        Direction.Up => walkUpSprites,
        //        Direction.Down => walkDownSprites,
        //        Direction.Left => walkLeftSprites,
        //        Direction.Right => walkRightSprites,
        //        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        //    };
        //}

        public void TakeDamage(int damage, GameObject source)
        {
            curHealth -= damage;
            Debug.Log(curHealth);

            if (curHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}