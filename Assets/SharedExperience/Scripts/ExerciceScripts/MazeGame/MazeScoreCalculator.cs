using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/Data/ScoreCalculators/MazeScoreCalc.asset", menuName = "ScriptableObjects/Score Calculators/Maze Exercise", order = 1)]
public class MazeScoreCalculator : IScoreCalculator
{
    private const float worstTimeInS = 60; //worst time possible in seconds (basically a 0%)

    public override ScoreCalculator.PerformanceSummary CalculateScore(List<GameObject> spawnedItemList, ClockHandler exercice_clock)
    {
        ScoreCalculator.PerformanceSummary summary = new ScoreCalculator.PerformanceSummary();
        summary.metricToValueList = new Dictionary<string, string>();
        summary.metricToValueList.Add("Time taken", exercice_clock.getCurrentTimeString());
        summary.metricToValueList.Add("total mirrors", spawnedItemList.Count + "");
        //TODO : use number of used items as metric

        //for now we just use time taken as a metric
        float timeTaken = exercice_clock.exercice_timer;
        int timePercent = 100 - Mathf.CeilToInt((timeTaken / worstTimeInS) * 100);

        //we set the total performance
        summary.totalPerformancePourcent = timePercent;
        return summary;
    }
}
