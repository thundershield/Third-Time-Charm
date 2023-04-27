using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemies
{
    public enum BehaviorState {
        idle, //default state, usually will have the enemy just move around at random
        combat,
        attacking,
        damaged,
        dead
    }
}
