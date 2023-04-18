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
        [SerializeField] private int aggroRange = 20; //how close the player needs to get to aggro this enemy
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
        private BehaviorState state = BehaviorState.idle;
        private Vector2 directionVec;
        private float distanceToTarget = 1000; //how far the target is, following the pathToTarget. Initialized to a high value so that default state will be idle
        private Rigidbody2D rb;
        private Seeker seeker;
        private Path pathToTarget; //This will always point towards one of the players. Used for determining distance
        private Path activePath; //This is the actual path the AI will follow
        private float wayPointDistanceThreshold = 0.5f; //How close we need to get to the target waypoint before getting a new target
        int targetWaypoint = 0;
        
        private void Start()
        {
            target = GameObject.Find("Player(Clone)").transform; //this is a temporary measure for testing purposes. Replace with proper system
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            seeker = GetComponent<Seeker>();
            //every quarter second update your path to the player
            InvokeRepeating("FindPathToTarget",0f,.25f);
            curHealth = maxHealth;
            seeker.StartPath(rb.position, target.position, TargetPathFound);
        }
        void FindPathToTarget(){
            if(seeker.IsDone()){//check that we're not already finding a path
                seeker.StartPath(rb.position, target.position, TargetPathFound);
            }
        }
        void TargetPathFound(Path newPath)
        {
            if(!newPath.error){
                pathToTarget = newPath;
                distanceToTarget = pathToTarget.GetTotalLength();
            }
        }

        private void FixedUpdate()
        {
            //Run whatever state the enemy is currently in
            switch(state){
                case BehaviorState.idle: 
                    IdleState();
                    break;
                case BehaviorState.combat: 
                    CombatState();
                    break;
                case BehaviorState.attacking: 
                    AttackingState();
                    break;
                case BehaviorState.damaged: 
                    DamagedState();
                    break;
                case BehaviorState.dead: 
                    DeadState();
                    break;
                default: //If there isn't a specific behavior we want, then just use the idle State.
                    IdleState();
                    break;
            }
            //if(pathToTarget != null){
            //    if (targetWaypoint >= pathToTarget.vectorPath.Count) {
            //        return;
            //    }
            //    if(distanceToTarget < 20){
            //        directionVec = ((Vector2)pathToTarget.vectorPath[targetWaypoint] - rb.position).normalized;//get the normalized vector pointing towards the current waypoint
            //        rb.MovePosition(rb.position + directionVec * maxSpeed * Time.fixedDeltaTime);
            //        //Increments the target waypoint if we are within distance
            //        if(((rb.position - (Vector2)pathToTarget.vectorPath[targetWaypoint]).magnitude)<wayPointDistanceThreshold) targetWaypoint++;
            //    }
            //}
        }
        //The AI uses a state machine, where the state functions handles switching states and the behavior functions handle the behavior associated with that state
        //The Behavior and state are seperated so that different enemies AIs can override the behavior without copying the state function
        private void IdleState(){
            //if the 
            if(distanceToTarget < aggroRange){
                state = BehaviorState.combat;
            }
            IdleBehavior();
        }
        private void IdleBehavior(){
        }
        private void CombatState(){
            if(distanceToTarget >= aggroRange){
                state = BehaviorState.idle;
            }
            CombatBehavior();
        }
        private void CombatBehavior(){
            if(pathToTarget != null){
                if (targetWaypoint >= pathToTarget.vectorPath.Count) {
                    return;
                }
                directionVec = ((Vector2)pathToTarget.vectorPath[targetWaypoint] - rb.position).normalized;//get the normalized vector pointing towards the current waypoint
                rb.MovePosition(rb.position + directionVec * maxSpeed * Time.fixedDeltaTime);
                //Increments the target waypoint if we are within distance
                if(((rb.position - (Vector2)pathToTarget.vectorPath[targetWaypoint]).magnitude)<wayPointDistanceThreshold) targetWaypoint++;
            }
        }
        private void AttackingState(){
            
        }
        private void AttackingBehavior(){
            
        }
        private void DamagedState(){
            
        }
        private void DamagedBehavior(){
            
        }
        private void DeadState(){
            
        }
        private void DeadBehavior(){
            
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