using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//TODO : find better way to do this? We have to create asset for each child but dunno sounds overkill
[CreateAssetMenu(fileName = "Assets/Resources/Data/ScoreCalculators/UtensilScoreCalc.asset", menuName = "ScriptableObjects/Score Calculators/Untensil Exercise", order = 1)]
public class UtensilScoreCalculator : IScoreCalculator
{
    public override string CalculateScore(List<GameObject> spawnedItemList, ClockHandler clock)
    {
        if (spawnedItemList == null || spawnedItemList.Count == 0)
        {
            return "No utensil was registered, no result to display";
        }

        string output = "";
        output += "Time Taken : " + clock.getCurrentTimeString() + "\n";
        //for now we just display the results as is
        int success_amount = 0; //calculates nubmer of utensils that have collided (triggering a success of the xercice)
        output += spawnedItemList
            .Select((utensil) => {
                UtensilBehaviour utensilBehaviour = utensil.GetComponent<UtensilBehaviour>();
                if (utensilBehaviour == null) return "";
                if (utensilBehaviour.resultPack.succeeded) success_amount++;
                return utensilBehaviour.resultPack.utensilName + " : " + utensilBehaviour.resultPack._distanceError + " / " + utensilBehaviour.resultPack._angleError;
            })
            .Aggregate((str1, str2) => str1 + "\n" + str2);

        return output;
    }
}
