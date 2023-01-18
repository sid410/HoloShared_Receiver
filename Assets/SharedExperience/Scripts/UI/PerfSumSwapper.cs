using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for a type of UI, swaps between the performance and summary
public class PerfSumSwapper : MonoBehaviour
{

    public GameObject ObjectiveHideable;
    public GameObject ScoreHideable;

    // Start is called before the first frame update

    private void OnEnable()
    {
        EventHandler.OnTutorialStarted += DisplayObjectives;
        EventHandler.OnExerciseOver += DisplayScores;
    }

    private void OnDisable()
    {
        EventHandler.OnTutorialStarted -= DisplayObjectives;
        EventHandler.OnExerciseOver -= DisplayScores;
    }


    private void DisplayObjectives(TutorialData td) => SwapUI(ScoreHideable, ObjectiveHideable);

    private void DisplayScores() => SwapUI(ObjectiveHideable, ScoreHideable);

    // Update is called once per frame
    void SwapUI(GameObject hidedObject, GameObject shownObject)
    {
        hidedObject.SetActive(false);
        shownObject.SetActive(true);
    }
}
