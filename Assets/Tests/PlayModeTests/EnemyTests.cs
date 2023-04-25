using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Assertions = UnityEngine.Assertions;
using UnityEngine.TestTools;
using Enemies;

public class EnemyTests
{
    //Creates a basic enemy for testing purposes
    public GameObject CreateEnemy(){
        GameObject enemyGameObject = new GameObject();
        enemyGameObject.AddComponent<Rigidbody2D>();
        enemyGameObject.AddComponent<BoxCollider2D>();
        enemyGameObject.gameObject.tag = "Enemy";
        var enemyController = enemyGameObject.AddComponent<EnemyController>();
        enemyController.InitializeEnemy();
        return enemyGameObject;
    }
    public GameObject CreatePlayer(){
        GameObject playerGameObject = new GameObject();
        playerGameObject.AddComponent<Rigidbody2D>();
        var playerControler = playerGameObject.AddComponent<meleeItem>();
        return playerGameObject;
    }
    public GameObject CreateMeleeItem(){
        GameObject itemGameObject = new GameObject();
        itemGameObject.AddComponent<Rigidbody2D>();
        var itemControler = itemGameObject.AddComponent<meleeItem>();
        return itemGameObject;
    }
    //White box testing for the TakeDamage function. Go throughs both the lethal and non-lethal damage branch
    // Check health after taking survivable damage
    [Test]
    public void SurvivableDamageHealth()
    {
        GameObject enemyGameObject = CreateEnemy();
        GameObject source = new GameObject();
        source.transform.position = new Vector3(1.0f, 1.0f, 0.0f);
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        int health = enemyController.GetCurHealth();
        int damage = health/2;
        enemyController.TakeDamage(damage, DamageType.physical, source);
        int newHealth = enemyController.GetCurHealth();
        Assert.AreEqual(health-damage, newHealth);
    }

    //Check the state after taking damage
    [Test]
    public void SurvivableDamageState()
    {
        GameObject enemyGameObject = CreateEnemy();
        GameObject source = new GameObject();
        source.transform.position = new Vector3(1.0f, 1.0f, 0.0f);
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        int damage = enemyController.GetCurHealth()/2;
        enemyController.TakeDamage(damage, DamageType.physical, source);
        Assert.AreEqual(BehaviorState.damaged, enemyController.GetState());
    }

    //Check the position after taking damage. It should be further away from the source
    [UnityTest]
    public IEnumerator SurvivableDamagePosition()
    {
        GameObject enemyGameObject = CreateEnemy();
        GameObject source = new GameObject();
        source.transform.position = new Vector3(1.0f, 1.0f, 0.0f);
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        int damage = enemyController.GetCurHealth()/2;
        Vector3 initialPos = enemyGameObject.transform.position;
        enemyController.TakeDamage(damage, DamageType.physical, source);
        yield return new WaitForSeconds(enemyController.GetKnockBackTime());
        Vector3 finalPos = enemyGameObject.transform.position;
        float initialDistance = (initialPos-source.transform.position).magnitude;
        float finalDistance = (finalPos-source.transform.position).magnitude;
        Assert.IsTrue(initialDistance<finalDistance);
    }
    // Check health after taking lethal damage
    [Test]
    public void LethalDamageHealth()
    {
        GameObject enemyGameObject = CreateEnemy();
        GameObject source = new GameObject();
        source.transform.position = new Vector3(1.0f, 1.0f, 0.0f);
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        int health = enemyController.GetCurHealth();
        int damage = health*2;
        enemyController.TakeDamage(damage, DamageType.physical, source);
        int newHealth = enemyController.GetCurHealth();
        Assert.AreEqual(0, newHealth);
    }

    //Check the state after taking lethal damage
    [Test]
    public void LethalDamageState()
    {
        GameObject enemyGameObject = CreateEnemy();
        GameObject source = new GameObject();
        source.transform.position = new Vector3(1.0f, 1.0f, 0.0f);
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        int damage = enemyController.GetCurHealth()*2;
        enemyController.TakeDamage(damage, DamageType.physical, source);
        Assert.AreEqual(BehaviorState.dead, enemyController.GetState());
    }
    //Check the position after taking damage. Killing an enemy shouldn't move it
    [UnityTest]
    public IEnumerator LethalDamagePosition()
    {
        GameObject enemyGameObject = CreateEnemy();
        GameObject source = new GameObject();
        source.transform.position = new Vector3(1.0f, 1.0f, 0.0f);
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        int damage = enemyController.GetCurHealth()*2;
        enemyController.TakeDamage(damage, DamageType.physical, source);
        yield return null;
        Vector3 initialPos = enemyGameObject.transform.position;
        yield return new WaitForSeconds(0.5f);
        Vector3 finalPos = enemyGameObject.transform.position;
        float initialDistance = (initialPos-source.transform.position).magnitude;
        float finalDistance = (finalPos-source.transform.position).magnitude;
        Assert.IsTrue((initialDistance-finalDistance)<0.1f);
    }
    //Acceptance testing for being in their attack state during attacks
    [UnityTest]
    public IEnumerator StateDuringAttack(){
        GameObject enemyGameObject = CreateEnemy();
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        enemyController.triggerAttack();
        yield return new WaitForSeconds(enemyController.GetAttackLength()/2);
        Assert.AreEqual(BehaviorState.attacking, enemyController.GetState());
    }
    //Acceptance testing for being in their idle state after attack
    [UnityTest]
    public IEnumerator StateAfterAttack(){
        GameObject enemyGameObject = CreateEnemy();
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        enemyController.triggerAttack();
        yield return new WaitForSeconds(enemyController.GetAttackLength()*2);
        Assert.AreEqual(BehaviorState.idle, enemyController.GetState());
    }
    //acceptence testing on IsPathPossible function
    //Testing IsPathPossible on the true state
    [Test]
    public void PathIsPossibleTest(){
        GameObject enemyGameObject = CreateEnemy();
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        Assert.IsTrue(enemyController.IsPathPossible(new Vector2(1.0f, 1.0f), enemyGameObject.transform.position));
    }
    //Testing IsPathPossible on the false state
    [Test]
    public void PathIsImpossibleTest(){
        GameObject enemyGameObject = CreateEnemy();
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        enemyGameObject.transform.position = new Vector3(10f,10f,0f);
        GameObject blocker = new GameObject();
        blocker.AddComponent<Rigidbody2D>();
        blocker.AddComponent<BoxCollider2D>();
        Assert.IsFalse(enemyController.IsPathPossible(new Vector2(0.0f, 0.0f), enemyGameObject.transform.position));
    }
    //Acceptance/Integration testing for enemy to recognize player and enter combat state
    [Test]
    public IEnumerator EnemyCombatDetection(){
        GameObject enemyGameObject = CreateEnemy();
        GameObject playerGameObject = CreatePlayer();
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        var playerController = enemyGameObject.GetComponent<PlayerControler>();
        enemyController.target = playerGameObject.transform;
        yield return new WaitForFixedUpdate();//Wait three updates so it has time to switch
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        Assert.AreEqual(BehaviorState.combat, enemyController.GetState());
    }
    //Integration Testing with the Items and Players hitting and being hit by enemies
    [Test]
    public void ItemDamageIntegrationTest(){
        GameObject enemyGameObject = CreateEnemy();
        GameObject playerGameObject = CreatePlayer();
        GameObject meleeItemGameObject = CreateMeleeItem();
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        var playerController = enemyGameObject.GetComponent<PlayerControler>();
        var meleeItemController = enemyGameObject.GetComponent<meleeItem>();
        meleeItemController.player = playerController;
        int damage = 10;
        meleeItemController.damage = damage;
        int health = enemyController.GetCurHealth();
        meleeItemController.use();
        Assert.AreEqual(enemyController.GetCurHealth()+damage, health);
    }
}
