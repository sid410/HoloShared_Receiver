using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Abstract class for anything that can take damage
public abstract class Damageable : MonoBehaviour
{
    [SerializeField] private int max_health = 3;
    protected int currentHealth;

    protected Team team;
    public virtual void Init(Team team, Vector3 startPosition)
    {
        currentHealth = max_health;
        this.team = team;
        this.gameObject.layer = SnowfightUtil.getTeamLayer(team);
        this.transform.position = startPosition;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (damage <= 0) Die();
    }

    protected abstract void Die();
}
