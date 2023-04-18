using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NpcState{
    stationary,
    moving,
    talking
}

public class NpcClass : MonoBehaviour
{
    public int hitpoints;
    public float npcSpeed;
    public string NpcName;
    public NpcState currentState;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
