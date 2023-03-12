using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Abstract class for anything that can take damage
public abstract class Damageable : MonoBehaviour
{
    public enum DamageableType
    {
        SOLDIER, STRUCTURE, CASTLE
    }

    public DamageableType unitType = DamageableType.SOLDIER; 

    [SerializeField] private int max_health = 3;
    [SerializeField] private bool invulnerable = false;
    protected int currentHealth;

    
    protected Team team;
    public virtual void Init(Team team, Vector3 startPosition)
    {
        currentHealth = max_health;
        this.team = team;
        this.gameObject.layer = SnowfightUtil.getTeamLayer(team);
        this.transform.localPosition = startPosition;
    }

    public virtual void TakeDamage(AttackType attackType) //Todo attack type specific behaviour
    {
        if (invulnerable) return;
        currentHealth -= 1;
        if (currentHealth <= 0) Die();
    }

    protected abstract void Die();
}
