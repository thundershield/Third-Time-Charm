using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour {

    public GameObject player;

    private Vector3 offset; 

    public bool isOpen = true;

    public MeshRenderer PLS;

    void Start() {
        offset = transform.position - player.transform.position;
        transform.position = player.transform.position;
    }

    void Update () {
        transform.position = player.transform.position + offset;

        if (Input.GetKeyDown("escape") && isOpen) {
            PLS.enabled = false;
            isOpen = false;
        }
        else if (Input.GetKeyDown("escape") && !isOpen) {
            PLS.enabled = true;
            isOpen = true;
        }
    }
}