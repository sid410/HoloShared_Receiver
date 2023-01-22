using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/Data/ObjectiveHandlers/mazeObjective.asset", menuName = "ScriptableObjects/Objective Handlers/Mze Exercise", order = 101)]
public class MazeObjectives : IObjectiveHandler
{

    public static int INIT_OBJECTIVE_INDEX = 1;
    public override ObjectiveUpdater.ObjectiveData getExerciseInitObjective()
    {
        return new ObjectiveUpdater.ObjectiveData(INIT_OBJECTIVE_INDEX, "Guide the Light beam towards the objective !");
    }

    public override ObjectiveUpdater.ObjectiveData getObjectiveFromSpawnedItem(GameObject spawnedItem)
    {
        return null;
    }
}
