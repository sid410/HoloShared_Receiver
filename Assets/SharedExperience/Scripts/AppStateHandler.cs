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
        NONE, TUTORIAL, EXERCICE, RESULTS,
    }

    public static AppState appState = AppState.NONE;


    private void Start()
    {
        appState = AppState.NONE;
    }

    #region subscriptions
    private void OnEnable()
    {
        EventHandler.Tutorial_started += TutorialPhase;
        EventHandler.Exercice_started += ExercisePhase;
        EventHandler.Exercice_over += ResultsPhase;
    }
    private void OnDisable()
    {
        EventHandler.Tutorial_started -= TutorialPhase;
        EventHandler.Exercice_started -= ExercisePhase;
        EventHandler.Exercice_over -= ResultsPhase;
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
