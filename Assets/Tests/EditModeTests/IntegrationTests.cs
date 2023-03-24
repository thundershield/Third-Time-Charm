using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

public class IntegrationTests
{
    private GameObject playerGameObject;
    private GameObject cameraGameObject;
    private CameraMovement cameraMovement;
    private PlayerMovement playerMovement;

    [SetUp]
    public void Setup()
    {
        playerGameObject = new GameObject();
        playerMovement = playerGameObject.AddComponent<PlayerMovement>();
        playerGameObject.AddComponent<Rigidbody2D>();
        playerGameObject.AddComponent<Animator>();
        playerMovement.playerSpeed = 5f;

        cameraGameObject = new GameObject();
        cameraMovement = cameraGameObject.AddComponent<CameraMovement>();
        cameraMovement.playerTarget = playerGameObject.transform;
        cameraMovement.cameraSmoothing = 0.1f;
    }

    [UnityTest]
    public IEnumerator CameraFollowsPlayer()
    {
        playerGameObject.transform.position = new Vector3(0f, 0f, 0f);
        cameraGameObject.transform.position = new Vector3(0f, 0f, -10f);

        yield return null;

        Assert.AreEqual(cameraGameObject.transform.position, new Vector3(0f, 0f, -10f));

        playerGameObject.transform.position = new Vector3(5f, 5f, 0f);

        yield return new WaitForSeconds(0.5f);

        Assert.AreEqual(cameraGameObject.transform.position, new Vector3(2.5f, 2.5f, -10f));
    }

    [UnityTest]
    public IEnumerator PlayerMovesWhenInputIsReceived()
    {
        playerGameObject.transform.position = new Vector3(0f, 0f, 0f);
        cameraGameObject.transform.position = new Vector3(0f, 0f, -10f);

        yield return null;

        playerMovement.playerSpeed = 5f;

        yield return new WaitForSeconds(0.1f);

    //    playerMovement.Update();
    //    playerMovement.playerMovementChange = new Vector3(1f, 0f, 0f);
     //   playerMovement.MoveCharacter();

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(playerGameObject.transform.position, new Vector3(5f, 0f, 0f));
    }

    [UnityTest]
    public IEnumerator CameraFollowsPlayerWhenPlayerMoves()
    {
        playerGameObject.transform.position = new Vector3(0f, 0f, 0f);
        cameraGameObject.transform.position = new Vector3(0f, 0f, -10f);

        yield return null;

        playerMovement.playerSpeed = 5f;

        yield return new WaitForSeconds(0.1f);

    //    playerMovement.Update();
    //    playerMovement.playerMovementChange = new Vector3(1f, 0f, 0f);
    //    playerMovement.MoveCharacter();

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(playerGameObject.transform.position, new Vector3(5f, 0f, 0f));
        Assert.AreEqual(cameraGameObject.transform.position, new Vector3(2.5f, 0f, -10f));
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(playerGameObject);
        Object.Destroy(cameraGameObject);
    }
}