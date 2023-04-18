﻿using System;
using UnityEngine;
using Unity.Netcode;

public class PlayerCamera : NetworkBehaviour
{
    private Transform _cameraTransform;
    
    private void Start()
    {
        var mainCamera = Camera.main;
        if (mainCamera is null) throw new NullReferenceException("No main camera for the player to access");
        _cameraTransform = mainCamera.transform;
    }

    private void Update()
    {
        var newPosition = transform.position;
        newPosition.z = _cameraTransform.position.z;
        _cameraTransform.position = newPosition;
    }
}