using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Enemies
{
    public class GhostController : EnemyController
    {
        [SerializeField] private float wanderRange = 3.0f;
        protected override void CombatBehavior(){
            if(pathToTargetLength > engagementDistance){
                activePath = null;
                targetWaypoint = MoveAlongPath(pathToTarget, targetWaypoint);
            }else{
                if(activePath == null){
                    if(seeker.IsDone()){
                        Vector2 destination = new Vector2(0,0);
                        int failedAttempts = 0;
                        while(failedAttempts < 10){
                            destination = Random.insideUnitCircle*wanderRange+rb.position;
                            destination = (Vector3)AstarPath.active.GetNearest(destination).node.position;//prevents the enemy from getting stuck on walls
                            float distance = ((Vector2)target.position-destination).magnitude;
                            if(distance>engagementDistance){
                                failedAttempts++;
                                continue;
                            }
                            RaycastHit2D ray = Physics2D.Raycast(destination, (destination-(Vector2)target.position),distance,LayerMask.GetMask("Player","Tilemap"));
                            if(ray.collider == null || ray.collider.gameObject.layer == LayerMask.NameToLayer("Tilemap")){
                                failedAttempts++;
                                continue;
                            }
                            if(!IsPathPossible(rb.position, destination)){
                                failedAttempts++;
                                continue;
                            }
                            break;
                        }
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
                        seeker.StartPath(rb.position, destination, ActivePathFound);
                    }
                }else{
                    if(curWaypoint < activePath.vectorPath.Count){
                        curWaypoint = MoveAlongPath(activePath, curWaypoint);
                    }else{
                        RaycastHit2D ray = Physics2D.Raycast(rb.position, (rb.position-(Vector2)target.position),LayerMask.GetMask("Player","Tilemap"));
                        if(ray.collider != null){// && ray.collider.gameObject.layer == LayerMask.NameToLayer("Player")){
                            StartCoroutine(AttackingBehavior((Vector2)target.position-rb.position));
                        }
                        activePath = null;
                    }
                }
            }
        }
        //returns a unit vector that is within a specified number of degrees of a line between two points
    }
}
