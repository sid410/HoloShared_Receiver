using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/Data/ObjectiveHandlers/mazeObjective.asset", menuName = "ScriptableObjects/Objective Handlers/Mze Exercise", order = 101)]
public class MazeObjectives : IObjectiveHandler
{

    public static int INIT_OBJECTIVE_INDEX = 1;

    public int totalGoals = 1;

    private int completedGoals = 0;


    public override bool CheckObjectiveDone(int index, GameObject caller)
    {
        completedGoals++;
        return completedGoals >= totalGoals;
    }

    //serves as an init as well
    public override ObjectiveUpdater.ObjectiveData getExerciseInitObjective()
    {
        EventHandler.OnBeforeMatlabDataReceived += ResetCompletedObjectives; //we reset completed objectives before every laser recast
        return new ObjectiveUpdater.ObjectiveData(INIT_OBJECTIVE_INDEX, "Guide the Light beam to the goal!");
    }

    public override ObjectiveUpdater.ObjectiveData getObjectiveFromSpawnedItem(GameObject spawnedItem)
    {
        return null;
    }

    public override void Cleanup()
    {
        EventHandler.OnBeforeMatlabDataReceived -= ResetCompletedObjectives;
        ResetCompletedObjectives();
    }

    private void ResetCompletedObjectives()
    {
        completedGoals = 0;
    }
}
