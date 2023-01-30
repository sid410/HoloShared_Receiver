using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IObjectiveHandler : ScriptableObject
{

    
    public abstract ObjectiveUpdater.ObjectiveData getExerciseInitObjective(); //called when an exercise is started, can return an objective
    public abstract ObjectiveUpdater.ObjectiveData getObjectiveFromSpawnedItem(GameObject spawnedItem); // called when a new item is created

    //called by the objective handler when an objective was called as completed. this returns true if the objective should be considered as completed
    public abstract bool CheckObjectiveDone(int index, GameObject caller);

    //cleanup before destroying the object
    public abstract void Cleanup();
}
