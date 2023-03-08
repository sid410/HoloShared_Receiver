using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Abstract class for projectiles
public abstract class Projectile : MonoBehaviour
{
    [Header("projectile data")]
    [SerializeField] private float projectileSpeed = 4f;
    [SerializeField] private AttackType projectileDamageType = AttackType.LIGHT;

    protected Damageable target = null;
    protected Rigidbody rb;

    public void Init(Team team, Damageable target)
    {
        this.target = target;
        this.gameObject.layer = SnowfightUtil.getTeamLayer(team);
    }

    private void FixedUpdate()
    {
        if (target == null) return;
        Vector3 direction = target.transform.position - transform.position;
        transform.position += direction * Time.fixedDeltaTime * projectileSpeed;
    }
}
