using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllySoldierBehaviour : SoldierBehaviour
{

    //either the soldier recovers or dies. Ally soldiers recover
    protected bool canRecover = false;
    protected float recoveryTime = 3f;

    protected float currentRecoveryCountdown;


    public override void Init(Team team, Vector3 startPosition)
    {
        base.Init(team, startPosition);
        SnowballDifficultyParamters gameSettings = SnowGameManager.Instance.gameParameters;

        //we prepare the settings
        enemyLayer =  1 << SnowfightUtil.getEnemyLayerMask();
        canRecover = gameSettings.allyUnitsRecover;
        recoveryTime = gameSettings.allySoliderRecoveryTime;
    }

    protected override void Update()
    {
        base.Update();
        ReCheckStructure();
    }

    //rechecks if the current structure is still available or we need to move
    private void ReCheckStructure()
    {
        if (allyStructure != null)
        {
            //structure is still well positionned, we do not need to hide
            if ((Mathf.Abs(allyStructure.transform.position.z - transform.position.z) < destinationDistanceMargin) && (Vector3.Magnitude(transform.position - allyStructure.transform.position) < destinationDistanceMargin * 5)) return;
        }

        isGarrisoned = false; //this soldier is exposed ! it will try to find cover
        allyStructure = null; //we reset the structure linked to this soldier
        List<SnowStructure> closestStructures = SnowGameManager.Instance.getClosestStructures(transform.position);
        if (closestStructures == null || closestStructures.Count == 0) return; //if no structure was found, return

        //we check the closest structure we can go to
        foreach (SnowStructure snowstructure in closestStructures)
        {
            Vector3 targetPosition = snowstructure.PositionNewSoldier();
            if (targetPosition != Vector3.zero)
            {
                isMoving = true;
                allyStructure = snowstructure;
                destination = targetPosition;
                return;
            }
        }
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }


    protected override void InitialMouvement()
    {
        List<SnowStructure> closestStructure = SnowGameManager.Instance.getClosestStructures(transform.position);

        isMoving = true;

        if (closestStructure != null) //if there are structures, we get the closest free structure
        {
            foreach (SnowStructure snowstructure in closestStructure)
            {
                Vector3 targetPosition = snowstructure.PositionNewSoldier();
                if (targetPosition != Vector3.zero)
                {
                    allyStructure = snowstructure;
                    destination = targetPosition;
                    return;
                }
            }

        }


        //we pick a random position between the limit and the current position if no structure is found or all are full.
        float zoneLimit = SnowGameManager.Instance.getAllyTerritoryLimit();
        float randomPrcent = Random.Range(0.4f, 0.9f);
        destination = new Vector3(transform.position.x, transform.position.y, transform.position.z + (zoneLimit - transform.position.z) * randomPrcent);

        return;
    }
}
