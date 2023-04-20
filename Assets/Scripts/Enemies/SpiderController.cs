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
        protected override void CombatBehavior(){
            if(pathToTargetLength > engagementDistance){
                targetWaypoint = MoveAlongPath(pathToTarget, targetWaypoint);
            }else{
                float distanceToTarget = ((Vector2)target.position - rb.position).magnitude;
                if(distanceToTarget < jumpRangeMax && distanceToTarget > jumpRangeMin){
                    state = BehaviorState.attacking;
                    StartCoroutine(AttackingBehavior((Vector2)target.position-rb.position));
                    activePath = null;
                }else if(activePath == null){
                    if(seeker.IsDone()){
                        //find the closest point to the target within range
                        destination = ((Vector2)target.position-rb.position).normalized*jumpRangeMin*1.4f*-1+(Vector2)target.position;
                        seeker.StartPath(rb.position, destination, ActivePathFound);
                    }
                }else{
                    //if the destination is outside of the jump range we need a new one
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
        protected override IEnumerator AttackingBehavior(Vector2 direction){
            if(direction.x > 0 ){
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }else{
                transform.localRotation = Quaternion.Euler(0,180,0);
            }
            animator.Play("Attack");
            yield return new WaitForSeconds(windupTime);
            timer = attackTime-windupTime;
            while(timer > 0){
                rb.MovePosition(rb.position + direction * 3 * Time.fixedDeltaTime);
                timer = timer - Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            rb.AddForce((transform.position-target.position).normalized*5, ForceMode2D.Impulse);
            hitBox.enabled = true;
            yield return new WaitForSeconds(attackTime-windupTime);
            state = BehaviorState.combat;
            AttackCleanUp();
        }
    }
}
