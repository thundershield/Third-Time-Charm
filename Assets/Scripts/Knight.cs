using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : NpcClass
{
    public Transform target;
    public Transform homePosition;
    public float findRadius;
    public float triggerRadius;

    // Start is called before the first frame update
    void Start()
    {
        currentState = NpcState.stationary;
        target = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        findDistance();
    }

    void findDistance()
    {
        if(Vector3.Distance(target.position, transform.position) <= findRadius &&
            Vector3.Distance(target.position, transform.position) > triggerRadius &&
            currentState != NpcState.talking)
        {
            if(currentState == NpcState.stationary || currentState == NpcState.moving)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, npcSpeed * Time.deltaTime);
                currentState = NpcState.moving;
                if(Vector3.Distance(target.position, transform.position) <= triggerRadius)
                {
                    currentState = NpcState.talking;
                }
            }
        }
    }

    private void changeState(NpcState newState)
    {
        if(currentState != newState)
        {
            currentState = newState;
        }
    }
}
