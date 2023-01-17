using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/**
 * Uses data received from kinect data to compare the location of the virtual object and real object and display score
 */
public class ScoreCalculator : MonoBehaviour
{


    
    [SerializeField] private TextMesh performance_summary_text;
    [SerializeField] private ClockHandler exercice_clock; //Instance to object that keeps track of the exercice time, the time can be used for results calculation
    private List<GameObject> registeredSpawnedItems = new List<GameObject>();

    private IScoreCalculator exerciseScoreCalculator;

    private void Start()
    {
        performance_summary_text.text = "Waiting for exercice to start";
    }


    private void OnEnable()
    {
        EventHandler.Tutorial_started += OnTutorialStarted;
        EventHandler.Exercise_Loaded += OnExerciseLoaded;
        EventHandler.NewItemSpawned += OnItemSpawned;
        EventHandler.Exercice_started += OnExerciseStarted;
        EventHandler.PostExerciceMatlabResultsReceived += CalculateAndDisplayResults; //we only calculate data after the final matlab results have come
        EventHandler.ResetApp += OnReset;
    }

    private void OnDisable()
    {
        EventHandler.Tutorial_started -= OnTutorialStarted;
        EventHandler.Exercise_Loaded -= OnExerciseLoaded;
        EventHandler.NewItemSpawned -= OnItemSpawned;
        EventHandler.Exercice_started -= OnExerciseStarted;
        EventHandler.PostExerciceMatlabResultsReceived -= CalculateAndDisplayResults; //we only calculate data after the final matlab results have come
        EventHandler.ResetApp -= OnReset;
    }

    private void OnTutorialStarted(TutorialData data) => performance_summary_text.text = "Tutorial in progress"; //we simply change the text displayed

    private void OnExerciseLoaded(FullExerciceData exercise) => exerciseScoreCalculator = exercise.scoreCalculator; //we updated the calculator to the one related to the exercise

    private void OnItemSpawned(GameObject item) => registeredSpawnedItems.Add(item); //we register spawned utensils

    private void OnExerciseStarted(ExerciceData exercice) => performance_summary_text.text = "Exercice is in progress !";

    //called when the exercice is 
    private void CalculateAndDisplayResults()
    {
        if (exerciseScoreCalculator == null)
        {
            performance_summary_text.text = "No score to display";
            return;
        }


        performance_summary_text.text = exerciseScoreCalculator.CalculateScore(registeredSpawnedItems, exercice_clock);


    }


    private void OnReset()
    {
        performance_summary_text.text = "Waiting for a new exercise !";
    }

    //TODO : Create an interface thaat implements the Result calculation, so that calculation can be independant and adapted depending on the exercice
}
