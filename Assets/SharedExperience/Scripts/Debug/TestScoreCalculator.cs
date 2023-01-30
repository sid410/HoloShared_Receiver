using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Assets/Resources/Data/ScoreCalculators/testScoreC.asset", menuName = "ScriptableObjects/Score Calculators/debug test calc", order = 10)]

//test class, all values are hardcoded
public class TestScoreCalculator : IScoreCalculator
{
    public override void AddStepResults(List<GameObject> spawnedItemList, ClockHandler exercice_clock)
    {
        //nothing
    }

    public override ScoreCalculator.PerformanceSummary CalculateScore()
    {
        ScoreCalculator.PerformanceSummary summary = new ScoreCalculator.PerformanceSummary();
        summary.metricToValueList = new Dictionary<string, string>();
        summary.metricToValueList.Add("Time taken", "5:00");
        summary.metricToValueList.Add("accuracy", "95%");

        //we set the total performance
        summary.totalPerformancePourcent = 98;
        return summary;
    }

    public override void Cleanup() { }

    public override void Init() { }
}
