using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//interface for calculating the results for the exercise


public abstract class IScoreCalculator : ScriptableObject
{
    

    public abstract void Init(); //initializes the object

    public abstract void Cleanup(); //called before item is discarded/replaced

    //calculate score and returns the string to display
    public abstract ScoreCalculator.PerformanceSummary CalculateScore(List<GameObject> spawnedItemList, ClockHandler exercice_clock);
}
