using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//Used for a type of UI, swaps between the performance and summary
public class PerfSumSwapper : MonoBehaviour
{

    public GameObject ObjectiveHideable;
    public GameObject ScoreHideable;
    public GameObject clockHideable;

    // Start is called before the first frame update

    private void OnEnable()
    {
        EventHandler.OnTutorialStarted += TutorialUI;
        EventHandler.OnExerciseStarted += ExerciseUI;
        EventHandler.OnExerciseOver += ExerciseOverUI;
    }

    private void OnDisable()
    {
        EventHandler.OnTutorialStarted -= TutorialUI;
        EventHandler.OnExerciseStarted -= ExerciseUI;
        EventHandler.OnExerciseOver -= ExerciseOverUI;
    }


    private void TutorialUI(TutorialData td) => SwapUI(new List<GameObject> { ScoreHideable, clockHideable, ObjectiveHideable }, 
                 new List<GameObject> { });

    private void ExerciseUI(ExerciceData ed) => SwapUI(new List<GameObject> { ScoreHideable },
                 new List<GameObject> { clockHideable, ObjectiveHideable });
    private void ExerciseOverUI() => SwapUI(new List<GameObject> { ObjectiveHideable },
                 new List<GameObject> { clockHideable, ScoreHideable });

    // hides and enables list of objects
    void SwapUI(List<GameObject> hidedObjects, List<GameObject> shownObjects)
    {
        hidedObjects.ForEach(ob => ob.SetActive(false));
        shownObjects.ForEach(ob => ob.SetActive(true));
    }
}
