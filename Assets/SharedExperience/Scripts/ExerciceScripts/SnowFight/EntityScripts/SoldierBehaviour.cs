using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoldierBehaviour : Damageable
{

    [Header("Soldier data, for attacking")]
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float windupTime = 3f;

}
