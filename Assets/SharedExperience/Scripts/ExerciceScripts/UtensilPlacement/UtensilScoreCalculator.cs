using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


//TODO : find better way to do this? We have to create asset for each child but dunno sounds overkill
[CreateAssetMenu(fileName = "Assets/Resources/Data/ScoreCalculators/UtensilScoreCalc.asset", menuName = "ScriptableObjects/Score Calculators/Untensil Exercise", order = 1)]
public class UtensilScoreCalculator : IScoreCalculator
{

    private const float WorstDistance = 5; //TO adjust
    private const float WorstAngle = 90;
    public override ScoreCalculator.PerformanceSummary CalculateScore(List<GameObject> spawnedItemList, ClockHandler clock)
    {
        ScoreCalculator.PerformanceSummary summary = new ScoreCalculator.PerformanceSummary();
        if (spawnedItemList == null || spawnedItemList.Count == 0)
        {
            return summary;
        }

        //No clock usage for now

        //for now we just display the results as is

        List<UtensilBehaviour.ResultPack> results = spawnedItemList
            .Select((utensil) => {
                UtensilBehaviour utensilBehaviour = utensil.GetComponent<UtensilBehaviour>();
                if (utensilBehaviour == null) return null;
                return utensilBehaviour.resultPack;
            })
            .ToList();


        int success_amount = 0; //calculates nubmer of utensils that have collided (triggering a success of the xercice)
        float totalAngularError = 0;
        float totalDistanceError = 0;
        foreach (UtensilBehaviour.ResultPack result in results)
        {
            if (result.succeeded) success_amount++;
            //.utensilName + " : " + utensilBehaviour.resultPack._distanceError + " / " + utensilBehaviour.resultPack._angleError
            totalAngularError += result._angleError;
            totalDistanceError += result._distanceError;
        }

        //we calculate the performance percent in int, we round up because we love giving freebies to our patients.
        int angularPourcent = 100 - Mathf.CeilToInt(((totalAngularError / results.Count) / WorstAngle) * 100);
        int distancePourcent = 100 - Mathf.CeilToInt(((totalAngularError / results.Count) / WorstDistance) * 100);
        int successPourcent = (success_amount / results.Count) * 100;

        //we set the summary of all metrics
        summary.metricToValueList = new Dictionary<string, string>();
        summary.metricToValueList.Add("Correctly placed", success_amount + "/" + results.Count);
        summary.metricToValueList.Add("Angle accuracy", angularPourcent + "%");
        summary.metricToValueList.Add("Distance accuracy", distancePourcent + "%");

        //we use the average of metrics with no weigh as total
        summary.totalPerformancePourcent = (successPourcent + distancePourcent + angularPourcent) / 3;

        return summary;
    }

    public override void Cleanup(){ }

    public override void Init(){ }
}
