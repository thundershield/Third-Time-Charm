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
        //[SerializeField] protected float frameLength = 0.2f;
        //[SerializeField] protected float directionChangeTime = 1.5f;
        [SerializeField] protected float maxSpeed = 2.0f;
        [SerializeField] protected int maxHealth = 50;
        [SerializeField] protected int aggroRange = 20;
        [SerializeField] protected float engagementDistance = 5.0f; //How close to get to the target before doing any more complex calculations. Many specific enemies use it
        [SerializeField] protected int idleRange = 5; //How far the AI will wander when idling
        [SerializeField] protected float knockBackTime = 0.25f; //how long the enemy will move back from being hit. This should be less then the stunTime
        [SerializeField] protected float stunTime = 0.5f; //how long the enemy will be stunned after being hit. This should be equal to the length of the stun animation
        [SerializeField] protected float knockBack = 3.0f; //Increases the speed the enemy will be knocked back at
        [SerializeField] protected float windupTime = 0.25f; //How long the enemy spends winding up before attacking
        [SerializeField] protected float attackTime = 1f; //how long it takes for an attack to fully complete, including windup time
        [SerializeField] protected float attackCooldown = 2f;
        protected bool isAttackOnCooldown = false;
        [SerializeField] protected float deathTime = 1f; //How long it takes to play the death animation
        [SerializeField] protected int curHealth = 50;
        [SerializeField] protected Collider2D hitBox;
        protected float timer = 0; //a basic timer variable used for various states
        protected bool justDamaged = false;

        [SerializeField] protected BehaviorState state = BehaviorState.idle;
        protected Vector2 directionVec;
        protected float pathToTargetLength = 1000; //how far the target is, following the pathToTarget. Initialized to a high value so that default state will be idle
        protected float activePathLength = 1000;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Rigidbody2D rb;
        [SerializeField] protected Seeker seeker;
        [SerializeField] protected Collider2D EnemyCollider;//Reference used to disable it when it dies
        protected Path pathToTarget; //This will always point towards one of the players. Used for determining distance
        protected Path activePath; //This is the actual path the AI will follow
        protected float wayPointDistanceThreshold = 0.20f; //How close we need to get to the target waypoint before getting a new target waypoint
        protected int curWaypoint = 0;
        protected int targetWaypoint = 0;
        protected Coroutine attack;
        
        protected void Start()
        {
            //target = GameObject.Find("Player(Clone)").transform; //this is a temporary measure for testing purposes. Replace with proper system
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            //rb = GetComponent<Rigidbody2D>();
            //seeker = GetComponent<Seeker>();
            //animator = GetComponent<Animator>();
            timer = Random.Range(1.0f, 4.0f);
            //update your path to the player 10 times a second. A* is pretty efficient and our grid is pretty small, so this isn't too expensive
            if(target!=null){
                InvokeRepeating("FindPathToTarget",0f,.1f);
            }
            curHealth = maxHealth;
        }
        protected void FindPathToTarget(){
            if(seeker.IsDone()&&(IsPathPossible(rb.position, target.position))){//check that we're not already finding a path
                seeker.StartPath(rb.position, target.position, TargetPathFound);
            }
        }
        protected void TargetPathFound(Path newPath)
        {
            if(!newPath.error){
                pathToTarget = newPath;
                pathToTargetLength = newPath.GetTotalLength();
                //targetWaypoint = 0;
            }
        }
        protected void ActivePathFound(Path newPath)
        {
            if(!newPath.error){
                activePath = newPath;
                activePathLength = newPath.GetTotalLength();
                curWaypoint = 0;
            }
        }

        protected void FixedUpdate()
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
        
        protected virtual void IdleState(){
            IdleBehavior();
            if(pathToTargetLength < aggroRange){
                state = BehaviorState.combat;
            }
        }
        //Default idle behavior is to just find a random point within idle range, move towards it, and then wait 1 to 4 seconds before repeating
        protected virtual void IdleBehavior(){
            if(timer > 0){//Wait until timer is finished
                timer = timer - Time.fixedDeltaTime;
                if(animator!=null) animator.SetBool("isMoving", false);
            }
            else if(activePath == null){//if there is no active path, try to create a new one
                if(seeker!=null && seeker.IsDone()){//don't cancel existing jobs
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
        protected virtual void CombatState(){
            CombatBehavior();
            if(pathToTargetLength >= aggroRange){
                state = BehaviorState.idle;
                timer = Random.Range(1.0f, 4.0f);
                CombatBehaviorCleanup();
            }
        }
        //default combat behavior is just to move towards the player, but this will be replaced most of the time
        protected virtual void CombatBehavior(){
            if(pathToTarget != null){
                if (targetWaypoint >= pathToTarget.vectorPath.Count) {
                    return;
                }
                targetWaypoint = MoveAlongPath(pathToTarget, targetWaypoint);
            }
        }
        //May be needed for some enemies
        protected virtual void CombatBehaviorCleanup(){
        }
        //AttackingState is purposefully empty as how it triggers varies by enemy
        protected virtual void AttackingState(){
        }
        //This should almost always be overwritten by the enemies unique controller
        protected virtual IEnumerator AttackingBehavior(Vector2 direction){
            animator.Play("Attack");
            //rb.velocity = Vector2.zero;
            yield return new WaitForSeconds(windupTime);
            hitBox.enabled = true;
            yield return new WaitForSeconds(attackTime-windupTime);
            state = BehaviorState.combat;
        }
        //This function should handle returning the enemy to a base state, turning off hitboxes and stuff
        //It's a seperate function so that it can be used when an attack is interrupted
        protected virtual void AttackCleanUp(){
            if(hitBox!=null)hitBox.enabled = false;
        }
        //Handles the delay before an enemy can attack again
        protected virtual IEnumerator AttackCooldown(){
            isAttackOnCooldown = true;
            yield return new WaitForSeconds(attackCooldown);
            isAttackOnCooldown = false;
        }
        //Damaged state is purposefully empty as DamagedBehavior takes care of the transition
        protected virtual void DamagedState(){
        }
        //Calculates and applies knockback
        protected virtual IEnumerator DamagedBehavior(Vector2 direction, float force){
            timer = knockBackTime;
            if(animator!=null){
                animator.Play("Damaged");
            }
            while(timer > 0){
                if(rb!=null)rb.MovePosition(rb.position + direction * knockBack * Time.fixedDeltaTime*force*timer/knockBackTime);
                timer = timer - Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(stunTime-knockBackTime);
            state = BehaviorState.combat;
        }
        //Dead state is purposefully empty as TakeDamage takes care of the transition
        protected virtual void DeadState(){
            
        }
        //plays the death animation and disables several parts of the enemy
        protected virtual IEnumerator DeadBehavior(){
            if(animator!=null)animator.Play("Death");
            if(EnemyCollider!=null)EnemyCollider.enabled = false;
            yield return new WaitForSeconds(deathTime);
            Destroy(gameObject);
        }
        //Moves towards a given waypoint on a path and returns whatever waypoint you are now on
        protected int MoveAlongPath(Path path, int waypoint){
            animator.SetBool("isMoving", true);
            directionVec = ((Vector2)path.vectorPath[waypoint] - rb.position).normalized;//get the normalized vector pointing towards the active waypoint
            rb.MovePosition(rb.position + directionVec * maxSpeed * Time.fixedDeltaTime);//move towards it. Time.fixedDeltaTime keeps speed consistent even with lag
            //Flip the enemy if they are moving left. The small gap around 0 prevents the enemy from rapidly changing its direction
            if(directionVec.x > 0.1 ){
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }else if(directionVec.x < -0.1){
                transform.localRotation = Quaternion.Euler(0,180,0);
            }
            //Increments the target waypoint if we reached it. Sadly since you can't just add a bool to an int it requires an if statement
            if(((rb.position - (Vector2)path.vectorPath[waypoint]).magnitude)<wayPointDistanceThreshold){
                return waypoint+1;
            }else{
                return waypoint;
            }
        }
        //Called from other objects to do damage to the enemy. Applies damage and knockback
        public void TakeDamage(int damage, DamageType type, GameObject source)
        {
            if(state != BehaviorState.dead){
                curHealth = curHealth - damage;
                if(attack != null){
                    StopCoroutine(attack);
                }
                activePath = null;//if we have an active path, we're probably going to want a new one after taking damage
                AttackCleanUp();
                if (curHealth>0){
                    state = BehaviorState.damaged;
                    Vector2 direction = (transform.position-source.transform.position).normalized;
                    if(direction.x < 0 ){
                        transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }else{
                        transform.localRotation = Quaternion.Euler(0,180,0);
                    }
                    StartCoroutine(DamagedBehavior(direction,damage/10));
                    justDamaged = true;
                }else{
                    curHealth = 0;
                    state = BehaviorState.dead;
                    StartCoroutine(DeadBehavior());
                }
            }
        }
        //A simple wrapper function that allows IsPathPossible to take two Vector2s
        public bool IsPathPossible(Vector2 start, Vector2 end){
            return PathUtilities.IsPathPossible(AstarPath.active.GetNearest(start).node,AstarPath.active.GetNearest(end).node);
        }
        public int GetCurHealth(){
            return curHealth;
        }
        public BehaviorState GetState(){
            return state;
        }
        public float GetKnockBackTime(){
            return knockBackTime;
        }
        public void InitializeEnemy(){
            rb = GetComponent<Rigidbody2D>();
        }
    }
}