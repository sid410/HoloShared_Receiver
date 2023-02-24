using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Keeps track of the app state (in exercice, results, tutorial ...)
 * Other functions use state to decide what happens
 * 
 */
public class AppStateHandler : MonoBehaviour
{
    public enum AppState
    {
        NONE = 0, TUTORIAL = 1, EXERCICE = 2, RESULTS = 3,
    }

    public static AppState appState = AppState.NONE;


    private void Start()
    {
        appState = AppState.NONE;
    }

    #region subscriptions
    private void OnEnable()
    {
        EventHandler.OnTutorialStarted += TutorialPhase;
        EventHandler.OnExerciseStarted += ExercisePhase;
        EventHandler.OnExerciseOver += ResultsPhase;
    }
    private void OnDisable()
    {
        EventHandler.OnTutorialStarted -= TutorialPhase;
        EventHandler.OnExerciseStarted -= ExercisePhase;
        EventHandler.OnExerciseOver -= ResultsPhase;
    }

    #endregion

    private void TutorialPhase(TutorialData td) => appState = AppState.TUTORIAL;

    private void ExercisePhase(ExerciceData ed) => appState = AppState.EXERCICE;

    private void ResultsPhase() => appState = AppState.RESULTS;


    private void OnReset()
    {
        appState = AppState.NONE;
    }
}
