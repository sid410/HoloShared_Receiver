using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Enemy snowball, behaves differently based on trajectory
public class EnemySnowball : Projectile
{


    private void OnCollisionEnter(Collision collision)
    {
        string hitTag = collision.gameObject.tag;

        switch (hitTag)
        {
            case SnowfightUtil.STRUCTURE_TAG:
            case SnowfightUtil.ENTITY_TAG:
                Damageable script = collision.gameObject.GetComponent<Damageable>();
                script.TakeDamage(this.projectileDamageType);
                break;
        }
    }
}
