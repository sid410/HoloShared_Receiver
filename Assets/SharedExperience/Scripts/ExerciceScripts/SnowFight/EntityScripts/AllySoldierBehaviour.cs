using System.Collections;
using System.Collections.Generic;
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
        canRecover = gameSettings.allyUnitsRecover;
        recoveryTime = gameSettings.allySoliderRecoveryTime;
    }


    protected override void Die()
    {
        throw new System.NotImplementedException();
    }

    protected override Damageable getTargetUnit()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitialMouvement()
    {
        throw new System.NotImplementedException();
    }
}
