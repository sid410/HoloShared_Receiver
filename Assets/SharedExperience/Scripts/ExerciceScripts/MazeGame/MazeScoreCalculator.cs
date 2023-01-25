using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/Data/ScoreCalculators/MazeScoreCalc.asset", menuName = "ScriptableObjects/Score Calculators/Maze Exercise", order = 1)]
public class MazeScoreCalculator : IScoreCalculator
{
    private const float worstTimeInS = 600; //worst time possible in seconds (basically a 0%)

    List<GameObject> spawnedObjects = new List<GameObject>(); //TODO : remove responsibility of saving objects from the main score calculator

    public override void Init()
    {
        spawnedObjects = new List<GameObject>();
        EventHandler.OnBeforeMatlabDataReceived += ResetItemList;
        EventHandler.OnItemSpawned += RegisterItem;
    }


    private void RegisterItem(GameObject item) => spawnedObjects.Add(item);
    private void ResetItemList() { spawnedObjects.Clear(); }

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

    public override void Cleanup()
    {
        EventHandler.OnBeforeMatlabDataReceived -= ResetItemList;
        EventHandler.OnItemSpawned -= RegisterItem;
    }
}
