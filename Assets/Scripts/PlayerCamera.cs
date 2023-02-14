using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public GameObject player;

     private Vector3 offset; 

    void Start() {
        offset = transform.position - player.transform.position;
        transform.position = player.transform.position;
    }

    void Update () {
        transform.position = player.transform.position + offset;
    }
}