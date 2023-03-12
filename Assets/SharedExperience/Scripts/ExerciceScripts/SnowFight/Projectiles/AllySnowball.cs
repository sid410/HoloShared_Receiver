using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllySnowball : Projectile
{

    //ally projectiles kinda ignore structures
    private void OnCollisionEnter(Collision collision)
    {
        string hitTag = collision.gameObject.tag;

        switch (hitTag)
        {
            case SnowfightUtil.ENTITY_TAG:
                Damageable script = collision.gameObject.GetComponent<Damageable>();
                script.TakeDamage(this.projectileDamageType);
                break;
        }
    }
}
