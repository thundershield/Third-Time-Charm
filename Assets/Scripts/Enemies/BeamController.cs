using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemies
{
    public class BeamController : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Collider2D hitBox;
        [SerializeField] private Texture[] beam;
        [SerializeField] private Texture[] beamFade;
        [SerializeField] private Texture[] beamFadeIn;
        [SerializeField] protected int damage = 10;
        private float animationTimer = 0;
        private float frameLength = 0.125f;
        private int animationStep = 0;
        private int numberOfFrames = 4;
        private Vector2 direction;
        private Vector2 endPoint;

        public void ActivateBeam(Vector2 direct, Vector2 position){
            if(direct.y>0)
                transform.rotation = Quaternion.Euler(0,0,Vector2.Angle(direct,Vector2.right));
            else
                transform.rotation = Quaternion.Euler(0,0,Vector2.Angle(direct,Vector2.right)*-1);
            direction = direct;
            transform.position=position;
            RaycastHit2D hit = Physics2D.Raycast(position, direction,40,LayerMask.GetMask("Tilemap"));
            lineRenderer.SetPosition(0,position);
            lineRenderer.SetPosition(1,hit.point);
            StartCoroutine(BeamAnimation());
        }

        private IEnumerator BeamAnimation(){
            //charge the beam
            animationStep = 1;
            lineRenderer.material.SetTexture("_MainTex",beamFadeIn[0]);
            while(animationStep<numberOfFrames){
                animationTimer=animationTimer+Time.fixedDeltaTime;
                if(animationTimer>frameLength){
                    lineRenderer.material.SetTexture("_MainTex",beamFadeIn[animationStep]);
                    animationStep++;
                    animationTimer = 0;
                }
                yield return new WaitForFixedUpdate();
            }
            //fire the beam
            hitBox.enabled = true;
            animationStep = 1;
            lineRenderer.material.SetTexture("_MainTex",beam[0]);
            while(animationStep<numberOfFrames*4){
                animationTimer=animationTimer+Time.fixedDeltaTime;
                if(animationTimer>frameLength){
                    lineRenderer.material.SetTexture("_MainTex",beam[animationStep%numberOfFrames]);
                    animationStep++;
                    animationTimer = 0;
                }
                yield return new WaitForFixedUpdate();
            }
            hitBox.enabled = false;
            //discharge the beam
            animationStep = 1;
            lineRenderer.material.SetTexture("_MainTex",beamFade[0]);
            while(animationStep<numberOfFrames){
                animationTimer=animationTimer+Time.deltaTime;
                if(animationTimer>frameLength){
                    lineRenderer.material.SetTexture("_MainTex",beamFade[animationStep]);
                    animationStep++;
                    animationTimer = 0;
                }
                yield return new WaitForFixedUpdate();
            }
            gameObject.SetActive(false);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")){
                collision.gameObject.GetComponent<PlayerControler>().TakeDamage(damage);
            }
        }
    }
}