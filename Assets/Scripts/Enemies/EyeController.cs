using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EyeController : EnemyController
    {
        private Vector2 direction;
        private Vector2 distance;
        private float aggroRangeX = 10;
        private float aggroRangeY = 8;
        [SerializeField] protected GameObject spinEffect;
        [SerializeField] protected GameObject beam;

        [SerializeField] protected SpriteRenderer sr;
        protected override void IdleState(){
            distance = (Vector2)target.position-rb.position;
            IdleBehavior();
            if(Mathf.Abs(distance.x)<aggroRangeX&&Mathf.Abs(distance.y)<aggroRangeY){
                state = BehaviorState.combat;
                timer = 0.5f;
            }
        }
        protected override void IdleBehavior(){
            direction = (Vector2)target.position-rb.position;
            direction=direction.normalized;
            animator.SetFloat("X", Mathf.Abs(direction.x));
            animator.SetFloat("Y", direction.y);
            if(direction.x<0){
                sr.flipX = true;
            }else{
                sr.flipX = false;
            }
        }
        protected override void CombatState(){
            distance = (Vector2)target.position-rb.position;
            CombatBehavior();
            IdleBehavior();
            if(Mathf.Abs(distance.x)>aggroRangeX||Mathf.Abs(distance.y)>aggroRangeY){
                state = BehaviorState.idle;
            }
        }
        protected override void CombatBehavior(){
            direction = (Vector2)target.position-rb.position;
            direction=direction.normalized;
            animator.SetFloat("X", Mathf.Abs(direction.x));
            animator.SetFloat("Y", direction.y);
            if(direction.x<0){
                sr.flipX = true;
            }else{
                sr.flipX = false;
            }
            if(timer>0){
                timer = timer - Time.fixedDeltaTime;
            }else{
                if(distance.magnitude<4){
                    attack = StartCoroutine(SpinAttack());
                    state = BehaviorState.attacking;
                }else{
                    attack = StartCoroutine(BeamAttack());
                    state = BehaviorState.attacking;
                }
            }
        }
        private IEnumerator SpinAttack(){
            animator.Play("SpinAttackStart");
            yield return new WaitForSeconds(windupTime);
            spinEffect.SetActive(true);
            yield return new WaitForSeconds(attackTime-windupTime);
            spinEffect.SetActive(false);
            timer = 1;
            state = BehaviorState.combat;
        }
        private IEnumerator BeamAttack(){
            float angle = Vector2.Angle(direction,Vector2.down);
            Vector2 spawnPoint;
            int layerOrder = 2;
            if(angle<27.5){
                spawnPoint = new Vector2(0,-0.1f);
                layerOrder = 4;
            }else if(angle<67.5){
                spawnPoint = new Vector2(.65f,-.1f);
                layerOrder = 4;
            }else if(angle<112.5){
                spawnPoint = new Vector2(1.2f,-.05f);
                layerOrder = 4;
            }else if(angle<152.5){
                spawnPoint = new Vector2(1.1f,0);
            }else{
                spawnPoint = new Vector2(0,0.5f);
            }
            if(direction.x<0)spawnPoint.x=spawnPoint.x*-1;
            Vector2 directionFromSpawn = (Vector2)target.position-(rb.position+spawnPoint);
            directionFromSpawn = directionFromSpawn.normalized;
            animator.SetBool("isBeaming",true);
            animator.Play("BeamAttack");
            beam.transform.position = spawnPoint;
            beam.GetComponent<LineRenderer>().sortingOrder = layerOrder;
            beam.SetActive(true);
            beam.GetComponent<BeamController>().ActivateBeam(directionFromSpawn,rb.position+spawnPoint);
            yield return new WaitForSeconds(3);
            state = BehaviorState.combat;
            animator.SetBool("isBeaming",false);
            timer = 1;
        }
        protected override void AttackCleanUp(){
            spinEffect.SetActive(false);
        }
        public override void TakeDamage(int damage, DamageType type, GameObject source){
            if(state != BehaviorState.dead){
                curHealth = curHealth - damage;
                if(curHealth>0){
                    StartCoroutine(redFlash());
                }else{
                    state = BehaviorState.dead;
                    if(attack!=null)StopCoroutine(attack);
                    StartCoroutine(DeadBehavior());
                }
            }
        }
        protected override IEnumerator DeadBehavior(){
            animator.Play("Death");
            EnemyCollider.enabled = false;
            yield return new WaitForSeconds(deathTime);
            var itemPrefab = GameObject.Find("Inventory").GetComponent<inventoryMenu>().itemPrefab;
            GameObject item =  Instantiate(itemPrefab, transform.position, Quaternion.identity);
            item.GetComponent<itemObject>().updateItem(6);
            Destroy(gameObject);
        }
        private IEnumerator redFlash(){
            Color tint = new Color(1,0.5f,0.5f,1);
            Color add = new Color(0,0.05f,0.05f,0);
            sr.color = tint;
            while(tint.b != 1){
                yield return new WaitForFixedUpdate();
                tint = tint+add;
                sr.color = tint;
            }
        }
    }
}
