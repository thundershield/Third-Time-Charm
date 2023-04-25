using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Enemies
{
    public class GhostController : EnemyController
    {
        [SerializeField] private float wanderRange = 3.0f;
        [SerializeField] private float minimumRange = 1.0f;
        [SerializeField] private GameObject projectile;
        protected override void CombatBehavior(){
            //close the distance before anything else
            if(pathToTargetLength > engagementDistance){
                if(activePath!=null) activePath = null;//if we have an active path, get rid of it so it can be regenerated later
                targetWaypoint = MoveAlongPath(pathToTarget, targetWaypoint);
            }else{
                if(activePath == null){
                    if(seeker.IsDone()){
                        //Try to find a location within wander range that is pathable and has a line of sight to the target
                        Vector2 destination = new Vector2(0,0);
                        int failedAttempts = 0;
                        while(failedAttempts < 10){
                            destination = Random.insideUnitCircle*wanderRange+rb.position;
                            destination = (Vector3)AstarPath.active.GetNearest(destination).node.position;//prevents the enemy from getting stuck on walls
                            if(!IsPathPossible(rb.position, destination)){
                                failedAttempts++;
                                continue;
                            }
                            float distance = ((Vector2)target.position-destination).magnitude;
                            if(distance>engagementDistance||distance<minimumRange){
                                failedAttempts++;
                                continue;
                            }
                            RaycastHit2D ray = Physics2D.Raycast(destination, ((Vector2)target.position-destination),distance,LayerMask.GetMask("Player","Tilemap"));
                            if(ray.collider == null || ray.collider.gameObject.layer == LayerMask.NameToLayer("Tilemap")){
                                failedAttempts++;
                                continue;
                            }
                            if(ray.collider)Debug.Log("Early Raycast hit layer"+ray.collider.gameObject.layer);
                            break;
                        }
                        //if we can't find such a location after 10 tries, instead just search for a nearby location and move there as  a fallback
                        if(failedAttempts >= 10){
                            while(true){
                                destination = Random.insideUnitCircle*wanderRange+rb.position;
                                destination = (Vector3)AstarPath.active.GetNearest(destination).node.position;//prevents the enemy from getting stuck on walls
                                if(!IsPathPossible(rb.position, destination)){
                                    continue;
                                }
                                break;
                            }
                        }
                        //Create the path for whatever point we found
                        seeker.StartPath(rb.position, destination, ActivePathFound);
                    }
                }else{
                    //If we haven't reached the end of the path, continue moving along it
                    if(curWaypoint < activePath.vectorPath.Count){
                        curWaypoint = MoveAlongPath(activePath, curWaypoint);
                    }else{
                        //If we have reached the end of the path, check to see if we can still hit the target from this position. There's not much sense in throwing a projectile into a wall
                        RaycastHit2D ray = Physics2D.Raycast(rb.position, ((Vector2)target.position)-rb.position,engagementDistance, LayerMask.GetMask("Player","Tilemap"));
                        if(ray.collider != null && ray.collider.gameObject.layer == 9){
                            state = BehaviorState.attacking;
                            attack = StartCoroutine(AttackingBehavior((Vector2)target.position-rb.position));
                        }
                        //Regardless of whether we could or not, get a new path
                        activePath = null;
                    }
                }
            }
        }
        protected override IEnumerator AttackingBehavior(Vector2 direction){
            animator.Play("Attack");
            if(direction.x > 0 ){
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }else{
                transform.localRotation = Quaternion.Euler(0,180,0);
            }
            yield return new WaitForSeconds(windupTime);
            //Create the projectile and launch it at the target
            GameObject newProjectile = Instantiate(projectile,hitBox.gameObject.transform.position,Quaternion.identity); 
            newProjectile.GetComponent<Rigidbody2D>().AddForce(((Vector2)target.position-rb.position).normalized*4,ForceMode2D.Impulse);
            yield return new WaitForSeconds(attackTime-windupTime);
            state = BehaviorState.combat;
            AttackCleanUp();
        }
    }
}