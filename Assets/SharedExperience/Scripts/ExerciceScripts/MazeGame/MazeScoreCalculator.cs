using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/Data/ScoreCalculators/MazeScoreCalc.asset", menuName = "ScriptableObjects/Score Calculators/Maze Exercise", order = 1)]
public class MazeScoreCalculator : IScoreCalculator
{
    public override string CalculateScore(List<GameObject> spawnedItemList, ClockHandler exercice_clock)
    {
        return "Time taken : " + exercice_clock.getCurrentTimeString();
    }
}
