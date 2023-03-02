using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Abstract class for projectiles
public abstract class Projectile : MonoBehaviour
{
    [Header("projectile data")]
    [SerializeField] private float projectileSpeed = 4f;
    [SerializeField] private AttackType projectileDamageType = AttackType.LIGHT;

    protected Vector3 direction;
    protected Rigidbody rb;

    public void Init(Team team, Vector3 spawnPosition, Vector3 direction)
    {
        rb = GetComponent<Rigidbody>();
        this.gameObject.layer = SnowfightUtil.getTeamLayer(team);
        this.transform.position = spawnPosition;

        rb.velocity = direction * projectileSpeed;
    }
}
