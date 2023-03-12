using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySoldierBehaviour : SoldierBehaviour
{


    public override void Init(Team team, Vector3 startPosition)
    {
        base.Init(team, startPosition);
        enemyLayer = 1 << SnowfightUtil.getAllyLayerMask(); //we get the enemy layer (which is the player's team)
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
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
                        //Vector3 targetPosition = new Vector3(hits[i].point.x, 0, hits[i].point.z);
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
            if (destination == Vector3.zero)
            {
                destination = new Vector3(transform.position.x, transform.position.y, SnowGameManager.Instance.getEnemyTerritoryLimit());
            }

            isMoving = true;
        }
    }

    // Start is called before the first frame update
}
