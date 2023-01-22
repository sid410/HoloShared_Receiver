using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//utensil placement objective handler, for special behaviour and generting objectives

[CreateAssetMenu(fileName = "Assets/Resources/Data/ObjectiveHandlers/UtensilObjectiveH.asset", menuName = "ScriptableObjects/Objective Handlers/Utensil Exercise", order = 100)]
public class UPObjective : IObjectiveHandler
{
    
    public override ObjectiveUpdater.ObjectiveData getExerciseInitObjective()
    {
        return null; //we have no special objective for start of exercise
    }

    public override ObjectiveUpdater.ObjectiveData getObjectiveFromSpawnedItem(GameObject spawnedItem)
    {
        UtensilBehaviour utensilBehaviour = spawnedItem.GetComponent<UtensilBehaviour>(); //we use this special script for UP spawned utensils
        if (utensilBehaviour == null) return null; //this shouldnt happen

        string objectiveText = "Set the " + utensilBehaviour.type.ToString() + " At the correct location";
        int itemId = utensilBehaviour.itemID;

        return new ObjectiveUpdater.ObjectiveData(itemId, objectiveText); //we simply must set the item at the correct location
    }
}
