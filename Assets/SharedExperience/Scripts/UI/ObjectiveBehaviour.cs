using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectiveUpdater;

/**
 * Handles the behaviour of an objective and its text
 * */
public class ObjectiveBehaviour : MonoBehaviour
{
    public TextMesh textdisplay;
    public GameObject DoneQuestDisplay;

    public ObjectiveData relatedObjective { get; private set; }
    public bool completed { private set; get; } //we treack if the quest was ever completed

    public void init(ObjectiveData objectiveData)
    {
        this.relatedObjective = objectiveData;
        this.DoneQuestDisplay.SetActive(false);
        this.completed = false;
        textdisplay.text = objectiveData.objectiveText;
    }


    //Sets the objective as done by slashing it
    public void SetObjectiveAsDone()
    {
        completed = true;
        DoneQuestDisplay.SetActive(true);
    }

    public void SetObjectiveAsFailed()
    {
        completed = false;
        DoneQuestDisplay.SetActive(false);
    }
}
