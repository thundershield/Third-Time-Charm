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
        var enemyController = enemyGameObject.AddComponent<EnemyController>();
        enemyController.InitializeEnemy();
        return enemyGameObject;
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
    //acceptence testing on IsPathPossible function
    [Test]
    public void PathIsPossibleTest(){
        GameObject enemyGameObject = CreateEnemy();
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        //enemyController.IsPathPossible(enemyGameObject.transform.position, New Vector2(1,1));
        Assert.IsTrue(enemyController.IsPathPossible(new Vector2(1.0f, 1.0f), enemyGameObject.transform.position));
    }
    //acceptence testing on IsPathPossible function
    [Test]
    public void PathIsImpossibleTest(){
        GameObject enemyGameObject = CreateEnemy();
        var enemyController = enemyGameObject.GetComponent<EnemyController>();
        enemyGameObject.transform.position = new Vector3(10f,10f,0f);
        GameObject blocker = new GameObject();
        blocker.AddComponent<Rigidbody2D>();
        blocker.AddComponent<BoxCollider2D>();
        //enemyController.IsPathPossible(enemyGameObject.transform.position, New Vector2(1,1));
        enemyController.IsPathPossible(new Vector2(0.0f, 0.0f), enemyGameObject.transform.position);
    }
}
