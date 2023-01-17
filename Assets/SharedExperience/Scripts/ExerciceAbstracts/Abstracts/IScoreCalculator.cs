using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//interface for calculating the results for the exercise


public abstract class IScoreCalculator : ScriptableObject
{
    //calculate score and returns the string to display
    public abstract string CalculateScore(List<GameObject> spawnedItemList, ClockHandler exercice_clock);
}
