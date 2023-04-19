using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Pathfinding;

namespace Enemies
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : MonoBehaviour
    {
        public Transform target; //will usually hold the Transform of the player
        //[SerializeField] private float frameLength = 0.2f;
        //[SerializeField] private float directionChangeTime = 1.5f;
        [SerializeField] private float maxSpeed = 20.0f;
        [SerializeField] private int maxHealth = 50;
        [SerializeField] private int aggroRange = 20; //how close the player needs to get to aggro this enemy
        [SerializeField] private int idleRange = 20; //How far the AI will wander when idling
        private int curHealth;
        private float timer = 0; //a basic timer variable used for various states

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
        private float activePathLength = 1000;
        private Rigidbody2D rb;
        private Seeker seeker;
        private Path pathToTarget; //This will always point towards one of the players. Used for determining distance
        private Path activePath; //This is the actual path the AI will follow
        private float wayPointDistanceThreshold = 0.25f; //How close we need to get to the target waypoint before getting a new target
        public int curWaypoint = 0;
        public int targetWaypoint = 0;
        
        private void Start()
        {
            target = GameObject.Find("Player(Clone)").transform; //this is a temporary measure for testing purposes. Replace with proper system
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            seeker = GetComponent<Seeker>();
            //update your path to the player 10 times a second. A* is pretty efficient and our grid is pretty small, so this isn't too expensive
            InvokeRepeating("FindPathToTarget",0f,.1f);
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
                distanceToTarget = newPath.GetTotalLength();
                //targetWaypoint = 0;
            }
        }
        void ActivePathFound(Path newPath)
        {
            if(!newPath.error){
                activePath = newPath;
                activePathLength = newPath.GetTotalLength();
                curWaypoint = 0;
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
        }
        //The AI uses a state machine, where the state functions handles switching states and the behavior functions handle the behavior associated with that state
        //The Behavior and state are seperated so that different enemies AIs can override the behavior without copying the state function
        
        private void IdleState(){
            IdleBehavior();
            if(distanceToTarget < aggroRange){
                state = BehaviorState.combat;
            }
        }
        //Default idle behavior is to just find a random point within idle range, move towards it, and then wait 1 to 4 seconds before repeating
        private void IdleBehavior(){
            if(timer > 0){//Wait until timer is finished
                timer = timer - Time.fixedDeltaTime;
            }
            else if(activePath == null){//if there is no active path, try to create a new one
                if(seeker.IsDone()){//don't cancel existing jobs
                    //find a point that is between idleRange and idleRange/2 units away. Repeats until such a point is found
                    Vector2 randomPoint = new Vector2(0,0);
                    while(randomPoint.magnitude<idleRange/2){
                        randomPoint = Random.insideUnitCircle*idleRange;
                    }
                    seeker.StartPath(rb.position, rb.position + randomPoint, ActivePathFound);//find a path to this new point
                }
            }
            else{
                if(activePathLength > idleRange){//if the path turned out to be too long when its actual distance was measured, ditch it
                    activePath = null;
                    return;
                }
                if(curWaypoint >= activePath.vectorPath.Count){//if our current waypoint is the last waypoint, we've finished moving along the path
                    timer = Random.Range(1.0f, 4.0f);
                    activePath = null;
                } else {
                    curWaypoint = MoveAlongPath(activePath, curWaypoint);
                }
            }
        }
        private void CombatState(){
            CombatBehavior();
            if(distanceToTarget >= aggroRange){
                state = BehaviorState.idle;
            }
        }
        private void CombatBehavior(){
            if(pathToTarget != null){
                if (targetWaypoint >= pathToTarget.vectorPath.Count) {
                    return;
                }
                if(distanceToTarget > 1){}
                    targetWaypoint = MoveAlongPath(pathToTarget, targetWaypoint);
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
        //Moves towards a given waypoint on a path and returns whatever waypoint you are now on
        private int MoveAlongPath(Path path, int waypoint){
            directionVec = ((Vector2)path.vectorPath[waypoint] - rb.position).normalized;//get the normalized vector pointing towards the active waypoint
            rb.MovePosition(rb.position + directionVec * maxSpeed * Time.fixedDeltaTime);//move towards it. Time.fixedDeltaTime keeps speed consistent even with lag
            //Increments the target waypoint if we reached it. Sadly since you can't just add a bool to an int it requires an if statement
            if(((rb.position - (Vector2)path.vectorPath[waypoint]).magnitude)<wayPointDistanceThreshold){
                return waypoint+1;
            }else{
                return waypoint;
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