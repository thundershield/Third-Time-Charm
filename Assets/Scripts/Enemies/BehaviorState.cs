using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemies
{
    public enum BehaviorState {
        idle, //default state, usually will have the enemy just move around at random
        combat, //when the enemy is engaged with the player
        attacking, //When performing an attack, like launching an arrow
        damaged,//triggered whenever the enemy takes damage
        dead//State to handle the death animation
    }
}
