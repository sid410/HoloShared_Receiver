using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballDifficultyParamters : ScriptableObject
{
    public int allySoldiers = 5; //how many soldiers the player gets
    public bool allyUnitsRecover = true; //maybe on hard no recovery
    public float allySoliderRecoveryTime = 3; //how much time friendly units take to recover
}
