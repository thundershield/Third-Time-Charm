//Unit tests for the camera movement script. Test fixture is broken up into setup and tests.
//Tests are broken up into arrange, act, and assert where the arrange sets the scene, the
//act calls the method being tested and the assert determines a pass or fail for that test.

//white box tests
//This is 100% branch coverage as both possible outcomes for the if statement are tested.

using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class CameraMovementTests
{
    private CameraMovement cameraMovement;
    private Transform playerTarget;

    [SetUp]
    public void SetUp()
    {
        GameObject cameraObject = new GameObject();
        cameraMovement = cameraObject.AddComponent<CameraMovement>();
        playerTarget = new GameObject().transform;
        cameraMovement.playerTarget = playerTarget;
        cameraMovement.cameraSmoothing = 1f;
    }

    [Test]
    public void CameraMovesTowardsTarget()
    {
        // Arrange
        Vector3 targetPosition = new Vector3(5f, 5f, 0f);
        playerTarget.position = targetPosition;

        Vector3 cameraStartPosition = new Vector3(0f, 0f, 0f);
        cameraMovement.transform.position = cameraStartPosition;

        // Act
        cameraMovement.LateUpdate();

        // Assert
        Assert.AreNotEqual(cameraStartPosition, cameraMovement.transform.position);
    }

    [Test]
    public void CameraMovesTowardsTargetX()
    {
        // Arrange
        Vector3 targetPosition = new Vector3(5f, 5f, 0f);
        playerTarget.position = targetPosition;

        Vector3 cameraStartPosition = new Vector3(0f, 0f, 0f);
        cameraMovement.transform.position = cameraStartPosition;

        // Act
        cameraMovement.LateUpdate();

        // Assert
        Assert.AreEqual(targetPosition.x, cameraMovement.transform.position.x);
    }

    [Test]
    public void CameraMovesTowardsTargetY()
    {
        // Arrange
        Vector3 targetPosition = new Vector3(5f, 5f, 0f);
        playerTarget.position = targetPosition;

        Vector3 cameraStartPosition = new Vector3(0f, 0f, 0f);
        cameraMovement.transform.position = cameraStartPosition;

        // Act
        cameraMovement.LateUpdate();

        // Assert
        Assert.AreEqual(targetPosition.y, cameraMovement.transform.position.y);
    }

    [Test]
    public void CameraMovesTowardsTargetZ()
    {
        // Arrange
        Vector3 targetPosition = new Vector3(5f, 5f, 0f);
        playerTarget.position = targetPosition;

        Vector3 cameraStartPosition = new Vector3(0f, 0f, 0f);
        cameraMovement.transform.position = cameraStartPosition;

        // Act
        cameraMovement.LateUpdate();

        // Assert
        Assert.AreEqual(cameraStartPosition.z, cameraMovement.transform.position.z);
    }

    [Test]
    public void CameraDoesNotMoveIfTargetHasNotMoved()
    {
        // Arrange
        Vector3 targetPosition = new Vector3(0f, 0f, 0f);
        playerTarget.position = targetPosition;

        Vector3 cameraStartPosition = new Vector3(5f, 5f, 0f);
        cameraMovement.transform.position = cameraStartPosition;

        // Act
        cameraMovement.LateUpdate();

        // Assert
        Assert.AreEqual(cameraStartPosition, cameraMovement.transform.position);
    }

    [Test]
    public void CameraDoesNotMoveIfTargetIsAtSamePositionAsCamera()
    {
        // Arrange
        Vector3 targetPosition = new Vector3(0f, 0f, 0f);
        playerTarget.position = targetPosition;

        Vector3 cameraStartPosition = new Vector3(0f, 0f, 0f);
        cameraMovement.transform.position = cameraStartPosition;

        // Act
        cameraMovement.LateUpdate();

        // Assert
        Assert.AreEqual(cameraStartPosition, cameraMovement.transform.position);
    }
}