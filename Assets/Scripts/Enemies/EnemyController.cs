using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using Pathfinding;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    {
        public Transform target; //will usually hold the Transform of the player
        //[SerializeField] private float frameLength = 0.2f;
        //[SerializeField] private float directionChangeTime = 1.5f;
        [SerializeField] private float maxSpeed = 20.0f;
        [SerializeField] private int maxHealth = 50;
        [SerializeField] private int aggroRange = 20; //how close the player needs to get to aggro this enemy
        [SerializeField] private int idleRange = 10; //How far the AI will wander when idling
        [SerializeField] private float knockBackTime = 0.25f; //how long the enemy will move back from being hit. This should be less then the stunTime
        [SerializeField] private float stunTime = 0.5f; //how long the enemy will be stunned after being hit. This should be equal to the length of the stun animation
        [SerializeField] private float knockBack = 3.0f; //how far back the enemy will be knocked after being hit
        [SerializeField] private int curHealth;
        private float timer = 0; //a basic timer variable used for various states
        private bool justDamaged = false;

        [SerializeField] private BehaviorState state = BehaviorState.idle;
        private Vector2 directionVec;
        private float distanceToTarget = 1000; //how far the target is, following the pathToTarget. Initialized to a high value so that default state will be idle
        private float activePathLength = 1000;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Seeker seeker;
        private Path pathToTarget; //This will always point towards one of the players. Used for determining distance
        private Path activePath; //This is the actual path the AI will follow
        private float wayPointDistanceThreshold = 0.20f; //How close we need to get to the target waypoint before getting a new target
        public int curWaypoint = 0;
        public int targetWaypoint = 0;
        
        private void Start()
        {
            target = GameObject.Find("Player(Clone)").transform; //this is a temporary measure for testing purposes. Replace with proper system
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            //rb = GetComponent<Rigidbody2D>();
            //seeker = GetComponent<Seeker>();
            //animator = GetComponent<Animator>();
            timer = Random.Range(1.0f, 4.0f);
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
            if(!newPath.error&&PathUtilities.IsPathPossible(newPath.path)){
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
            if(justDamaged){
                state = BehaviorState.damaged;
                justDamaged = false;
            }
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
                animator.SetBool("isMoving", false);
            }
            else if(activePath == null){//if there is no active path, try to create a new one
                if(seeker.IsDone()){//don't cancel existing jobs
                    Vector2 randomPoint;
                    GraphNode randomNode;
                    do{
                        //creates a point that is between idleRange/2 and idleRange units away
                        randomPoint = Random.insideUnitCircle*idleRange/2;
                        randomPoint = randomPoint + randomPoint.normalized*idleRange/2+rb.position;
                        randomNode = AstarPath.active.GetNearest(randomPoint).node;//Find the closest node to that point
                        //If the node is unreachable, abandon it and retry
                    }while(!randomNode.Walkable || !PathUtilities.IsPathPossible((AstarPath.active.GetNearest(rb.position).node),randomNode));
                    seeker.StartPath(rb.position, (Vector3)randomNode.position, ActivePathFound);//find a path to this new point
                }
            }
            else{
                if(activePathLength > idleRange&&PathUtilities.IsPathPossible(activePath.path)){//if the path turned out to be too long when its actual distance was measured, ditch it
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
                timer = Random.Range(1.0f, 4.0f);
            }
        }
        private void CombatBehavior(){
            if(pathToTarget != null){
                if (targetWaypoint >= pathToTarget.vectorPath.Count) {
                    return;
                }
                targetWaypoint = MoveAlongPath(pathToTarget, targetWaypoint);
            }
        }
        private void AttackingState(){
            
        }
        private void AttackingBehavior(){
            
        }
        private void DamagedState(){
            //Damaged state is purposefully empty as DamagedBehavior takes care of the transition
        }
        IEnumerator DamagedBehavior(){
            yield return new WaitForSeconds(knockBackTime);
            rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(stunTime-knockBackTime);
            state = BehaviorState.combat;
            rb.mass = 1;
        }
        private void DeadState(){
            
        }
        private void DeadBehavior(){
            
        }
        //Moves towards a given waypoint on a path and returns whatever waypoint you are now on
        private int MoveAlongPath(Path path, int waypoint){
            animator.SetBool("isMoving", true);
            directionVec = ((Vector2)path.vectorPath[waypoint] - rb.position).normalized;//get the normalized vector pointing towards the active waypoint
            rb.MovePosition(rb.position + directionVec * maxSpeed * Time.fixedDeltaTime);//move towards it. Time.fixedDeltaTime keeps speed consistent even with lag
            if(directionVec.x > 0 ){
                transform.localScale = new Vector3(1,1,1);
            }else{
                transform.localScale = new Vector3(-1,1,1);
            }
            //Increments the target waypoint if we reached it. Sadly since you can't just add a bool to an int it requires an if statement
            if(((rb.position - (Vector2)path.vectorPath[waypoint]).magnitude)<wayPointDistanceThreshold){
                return waypoint+1;
            }else{
                return waypoint;
            }
        }
        public void TakeDamage(int damage, GameObject source)
        {
            curHealth -= damage;
            Debug.Log(curHealth);

            if (curHealth>0){
                animator.Play("Damaged");
                Vector2 direction = (transform.position-source.transform.position).normalized;
                rb.AddForce((transform.position-source.transform.position).normalized*knockBack*damage/10, ForceMode2D.Impulse);
                rb.mass = 5;
                StartCoroutine(DamagedBehavior());
                justDamaged = true;
            }else{
                Destroy(gameObject);
            }
        }
    }
}