using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/Data/ScoreCalculators/MazeScoreCalc.asset", menuName = "ScriptableObjects/Score Calculators/Maze Exercise", order = 1)]
public class MazeScoreCalculator : IScoreCalculator
{
    private const float worstTimeInS = 600f; //worst time possible in seconds (basically a 0%)
    private const float bestTime = 30f;

    List<GameObject> spawnedObjects = new List<GameObject>(); 

    //a class to track each step's user performance
    private class StepResultData
    {
        public int totalUsedItems;
        public float timeTaken;
        public int performancePercent;
    }

    private List<StepResultData> allResults = new List<StepResultData>();
    public override void Init()
    {
        spawnedObjects = new List<GameObject>();
        EventHandler.OnBeforeMatlabDataReceived += ResetItemList;
        EventHandler.OnItemSpawned += RegisterItem;
    }


    private void RegisterItem(GameObject item) => spawnedObjects.Add(item);
    private void ResetItemList() { spawnedObjects.Clear(); }


    //called when an exercise step to store the results until the final score calculation phase
    public override void AddStepResults(ClockHandler exercice_clock)
    {
        Debug.Log("currently adding step results : time is " + exercice_clock.exercice_timer);
        StepResultData stepResult = new StepResultData();
        stepResult.totalUsedItems = spawnedObjects.Count;
        stepResult.timeTaken = exercice_clock.exercice_timer;
        float fixedTimeTaken = (stepResult.timeTaken - bestTime) < 0 ? 0 : stepResult.timeTaken - bestTime; //we give 100% if user made it in 20 seconds
        stepResult.performancePercent = 100 - Mathf.CeilToInt((fixedTimeTaken / worstTimeInS) * 100);
        allResults.Add(stepResult);

    }


    //calculates the results based on steps
    public override ScoreCalculator.PerformanceSummary CalculateScore()
    {
        ScoreCalculator.PerformanceSummary summary = new ScoreCalculator.PerformanceSummary();
        summary.metricToValueList = new Dictionary<string, string>();
        summary.metricToValueList.Add("Time taken", Util.FormatTimeFloat(allResults.Select(result => result.timeTaken).Aggregate((t1, t2) => t1 + t2)));
        summary.metricToValueList.Add("total mirrors", "" + allResults.Select(result => result.totalUsedItems).Aggregate((t1, t2) => t1 + t2));
        //TODO : use number of used items as metric

        //we set the total performance
        summary.totalPerformancePourcent = allResults.Select(result => result.performancePercent).Aggregate((t1, t2) => t1 + t2) / allResults.Count;
        return summary;
    }

    public override void Cleanup()
    {
        EventHandler.OnBeforeMatlabDataReceived -= ResetItemList;
        EventHandler.OnItemSpawned -= RegisterItem;
    }
}
