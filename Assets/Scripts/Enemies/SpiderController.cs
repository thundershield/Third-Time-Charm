using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class SpiderController : EnemyController
    {
        [SerializeField] protected float engagementDistance = 5.0f;//the spider will close to engagement distance before considering anything else
        [SerializeField] protected float jumpRangeMax = 3.0f;
        [SerializeField] protected float jumpRangeMin = 2.0f;
        private Vector2 destination;
        //The intended combat behavior for spiders is to get within a specified range of the player, jump at them, and repeat
        protected override void CombatBehavior(){
            //before doing anything complex just close distance
            if(pathToTargetLength > engagementDistance){
                targetWaypoint = MoveAlongPath(pathToTarget, targetWaypoint);
            }else{
                //check if the player is within range of your jump attack and execute it if they are
                float distanceToTarget = ((Vector2)target.position - rb.position).magnitude;
                if(distanceToTarget < jumpRangeMax && distanceToTarget > jumpRangeMin){
                    if(isAttackOnCooldown){
                        animator.SetBool("isMoving", false);
                    }else{
                        state = BehaviorState.attacking;
                        StartCoroutine(AttackingBehavior((Vector2)target.position-rb.position));
                        StartCoroutine(AttackCooldown());
                        activePath = null;
                    }
                //If you aren't in range and don't have a path, make one that will get you in range
                }else if(activePath == null){
                    if(seeker.IsDone()){
                        destination = GetOffAngleVector(target.position,rb.position)*jumpRangeMin*1.4f*-1+(Vector2)target.position;
                        if(IsPathPossible(rb.position, destination))//if the path isn't valid, try again
                            seeker.StartPath(rb.position, destination, ActivePathFound);
                    }
                }else{
                    //if our originally selected destination has moved out of range, abandon it
                    float difference = ((Vector2)target.position-destination).magnitude;
                    if(difference>jumpRangeMax||difference<jumpRangeMin){
                        activePath = null;
                    }else if(curWaypoint < activePath.vectorPath.Count){
                        //Otherwise just move along this path
                        curWaypoint = MoveAlongPath(activePath, curWaypoint);
                    }
                }
            }
        }
        //returns a unit vector that is within 45 degrees of a line between two points
        private Vector2 GetOffAngleVector(Vector2 origin, Vector2 endPoint){
            Vector2 shiftedVector = (origin-endPoint).normalized;
            shiftedVector = Quaternion.Euler(0,0,Random.Range(-45,45))*shiftedVector;
            return shiftedVector;
        }
        //The spiders attack consists of a lunging jump in a specified direction
        protected override IEnumerator AttackingBehavior(Vector2 direction){
            if(direction.x > 0 ){
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }else{
                transform.localRotation = Quaternion.Euler(0,180,0);
            }
            animator.Play("Attack");
            yield return new WaitForSeconds(windupTime);
            timer = attackTime-windupTime;
            hitBox.enabled = true;
            while(timer > 0){
                rb.MovePosition(rb.position + direction * 3 * Time.fixedDeltaTime);
                timer = timer - Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            state = BehaviorState.combat;
            AttackCleanUp();
        }
    }
}
