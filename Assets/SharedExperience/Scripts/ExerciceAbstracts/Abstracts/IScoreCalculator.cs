using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//interface for calculating the results for the exercise


public abstract class IScoreCalculator : ScriptableObject
{
    

    public abstract void Init(); //initializes the object

    public abstract void Cleanup(); //called before item is discarded/replaced

    //saves the results from a previous step
    public abstract void AddStepResults(ClockHandler exercice_clock);

    //calculate the score from all step results
    public abstract ScoreCalculator.PerformanceSummary CalculateScore();
}
