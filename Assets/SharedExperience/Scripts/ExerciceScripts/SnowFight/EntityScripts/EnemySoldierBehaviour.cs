using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySoldierBehaviour : SoldierBehaviour
{
    protected override void Die()
    {
        throw new System.NotImplementedException();
    }

    protected override Damageable getTargetUnit()
    {
        //look in a tight cone (x position difference must be tight)
        //check closest object, if structure & has soldier => hit solider but block damage, 
        //if soldier is unprotected => hit soldier but no damage blocking

        int layer = 1 << SnowfightUtil.getAllyLayerMask(); //we want to only detect enemy units
        List<RaycastHit> targetsInRange = Physics.BoxCastAll(transform.position, new Vector3(halfBoxTarget, 0.1f, 2f), transform.forward, Quaternion.identity, 2f, layer).ToList();

        float soliderZposition = transform.position.z;
        targetsInRange.Sort((t1, t2) => t1.transform.position.z.CompareTo(t2.transform.position.z)); //we sort by distance, we start going one by one until we hit a real block

        Damageable target = null;
        foreach(RaycastHit targetinRange in targetsInRange)
        {
            Damageable targetObj = targetinRange.transform.gameObject.GetComponent<Damageable>();
            if (targetObj == null) continue;

            if (targetObj.unitType == DamageableType.STRUCTURE)
            {
                SoldierBehaviour targetSoldier = ((SnowStructure)targetObj).getGarrisonnedSoldier();
                if (targetSoldier == null) continue; //if no soldier is hiding behind the structure, we check the next target in line
                return targetSoldier; //we return the hidden soldier otherwise
            } else
            {
                //case we found a soldier or the castle (the user which should be max distance)
                return targetObj;
            }
            
        } //TODO : register the castle for ease of access in case of raycast miss
        return null;
    }

    protected override void InitialMouvement()
    {
        RaycastHit[] hits;
        int layer = 1 << SnowfightUtil.getEnemyLayerMask();

        //we get all ally units in the path
        hits = Physics.RaycastAll(new Vector3(transform.position.x, 0, transform.position.z), transform.TransformDirection(transform.forward), 1f, layer);
        //we now find the first structure if any that was Hit, if we found one we go hide behind it
        if (hits != null && hits.Length > 0)
        {
            
            for (int i = 0; i< hits.Length; i++)
            {
                GameObject hit = hits[i].collider.gameObject;

                //we check if it's a structure
                if (hit != null && hit.tag.Equals(SnowfightUtil.STRUCTURE_TAG))
                {
                    SnowStructure hitStructure = hit.GetComponent<SnowStructure>();
                    if (hitStructure != null)
                    {
                        //we ask if there structure still has place
                        Vector3 targetPosition = hitStructure.PositionNewSoldier();

                        //the structure accepts soldiers ! this soldier will move to it
                        if (targetPosition != Vector3.zero)
                        {
                            destination = targetPosition; //we go to the destination given to us by the structure
                            allyStructure = hitStructure; //we keep track of the structure
                            break; //we break the loop
                        }
                    }
                    
                }
            }

            //we check if we found any structure to hide behind, otherwise the soldier goes on the front and shoots
            if (allyStructure == null)
            {
                destination = new Vector3(transform.position.x, transform.position.y, SnowGameManager.Instance.getEnemyTerritoryLimit());
            }

            isMoving = true;
        }
    }

    // Start is called before the first frame update
}
