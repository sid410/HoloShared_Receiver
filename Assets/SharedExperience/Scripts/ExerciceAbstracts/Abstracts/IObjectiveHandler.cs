using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IObjectiveHandler : ScriptableObject
{

    public abstract ObjectiveUpdater.ObjectiveData getExerciseInitObjective(); //called when an exercise is started, can return an objective
    public abstract ObjectiveUpdater.ObjectiveData getObjectiveFromSpawnedItem(GameObject spawnedItem); // called when a new item is created
}
