//acceptance tests because it tests whether or not chest functions perform properly 
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;

public class ChestTests {

    [Test]
    public void PlayerEntersTriggerArea_ChestIsTriggeredAndDialogBoxIsNotActive() {
        // Arrange
        var chest = new GameObject().AddComponent<Chest>();
        var player = new GameObject();
        var collider = chest.gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        player.transform.position = new Vector3(0, 0, 0);
        collider.transform.position = new Vector3(1, 1, 0);

        // Act
        collider.gameObject.SendMessage("OnTriggerEnter2D", player.GetComponent<Collider2D>());

        // Assert
        Assert.IsTrue(chest.chestTriggered);
        Assert.IsFalse(chest.chestDialogBox.activeSelf);
    }

    [Test]
    public void PlayerExitsTriggerArea_ChestIsNotTriggeredAndDialogBoxIsNotActive() {
        // Arrange
        var chest = new GameObject().AddComponent<Chest>();
        var player = new GameObject();
        var collider = chest.gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        player.transform.position = new Vector3(0, 0, 0);
        collider.transform.position = new Vector3(1, 1, 0);
        collider.gameObject.SendMessage("OnTriggerEnter2D", player.GetComponent<Collider2D>());

        // Act
        collider.gameObject.SendMessage("OnTriggerExit2D", player.GetComponent<Collider2D>());

        // Assert
        Assert.IsFalse(chest.chestTriggered);
        Assert.IsFalse(chest.chestDialogBox.activeSelf);
    }

    [Test]
    public void PlayerPressesSpacebarWhenInTriggerArea_ChestDialogBoxIsActivatedAndTextIsDisplayed() {
        // Arrange
        var chest = new GameObject().AddComponent<Chest>();
        var player = new GameObject();
        var collider = chest.gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        player.transform.position = new Vector3(0, 0, 0);
        collider.transform.position = new Vector3(1, 1, 0);
        collider.gameObject.SendMessage("OnTriggerEnter2D", player.GetComponent<Collider2D>());

        // Set up expected values
        chest.chestNameString = "Chest Name";
        chest.chestTextString = "Chest Text";

        // Act
     //   chest.Update();
        chest.npcName = new GameObject().AddComponent<Text>();
        chest.chestDialogText = new GameObject().AddComponent<Text>();
        chest.chestDialogBox.SetActive(true);

        // Assert
        Assert.IsTrue(chest.chestDialogBox.activeSelf);
        Assert.AreEqual(chest.chestNameString, chest.npcName.text);
        Assert.AreEqual(chest.chestTextString, chest.chestDialogText.text);
    }
}
*/