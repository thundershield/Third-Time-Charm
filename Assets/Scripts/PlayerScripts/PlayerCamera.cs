using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    private GameObject player;

    private Vector3 offset; 

    public void startGame() {
        offset = transform.position - player.transform.position;
        transform.position = player.transform.position;
    }

    void Update () {
        transform.position = player.transform.position + offset;
    }

    public void setCamera(GameObject playerObject) {
        player = playerObject;
    }
}