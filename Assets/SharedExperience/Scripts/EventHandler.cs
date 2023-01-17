using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ExerciceData;
using static TutorialData;

public class EventHandler : MonoBehaviour
{
    public static EventHandler Instance { get; private set; }

    //Item spawning and objective handling (DEPRECATED MOSTLY)
    public static event Action<UtensilBehaviour> ObjectiveCompleted;
    public static event Action<UtensilBehaviour> ObjectiveFailed; //when objective is failed after being completed (item moved out
    public static event Action<UtensilBehaviour> NewUtensilSpawned;

    public static event Action<Vector3, Quaternion> calibrationDone; //called after the calibration using vuforia is done.

    public static event Action<int, GameObject> ObjectiveCompletedU;
    public static event Action<int, GameObject> ObjectiveFailedU; //when objective is failed after being completed (item moved out
    public static event Action<GameObject> NewItemSpawned;

    //DATA LOADING STEP
    public static event Action<FullExerciceData> Exercise_Loaded;
    //Tutorial Steps
    public static event Action Tutorial_over;
    public static event Action<TutorialData> Tutorial_started;
    public static event Action<TutorialStep> Tutorial_step;

    //exercice steps
    public static event Action Exercice_over;
    public static event Action<ExerciceData> Exercice_started;
    public static event Action<ExerciceStep> Exercice_step;
    //kinect events
    public static event Action MatlabResultsReceived; //called every time we get matlab resutls
    public static event Action PostExerciceMatlabResultsReceived; //this is called for matlab results that happen after an exercice is over. Which basically marks the official end of the exercice

    //debug events
    public static event Action<string> LogDataUpdated;
    public static event Action ResetApp;


    private void Awake()
    {
        if (Instance != null) return;
        Instance = this;
    }


    public virtual void OnExerciseLoaded(FullExerciceData loadedExercise) // called when an exercise is loaded, all different parts are passed to all scripts
    { 
        Exercise_Loaded?.Invoke(loadedExercise);
    }

    #region Tutorial step events


    public virtual void OnTutorialOver() //called to inform that the current exercice is over.
    {
        Tutorial_over?.Invoke();
    }

    public virtual void OnTutorialStarted(TutorialData tutorialData) // called when an exercice is started
    {
        Tutorial_started?.Invoke(tutorialData);
    }

    public virtual void OnNewTutorialStepStarted(TutorialStep tutorialStep)
    {
        Tutorial_step?.Invoke(tutorialStep);
    }

    #endregion
    #region Exercice step events
    public virtual void OnExerciceOver() //called to inform that the current exercice is over.
    {
        Exercice_over?.Invoke();
    }

    public virtual void OnExerciceStarted(ExerciceData exerciseData)// ExerciceData exerciceData // called when an exercice is started
    {
        Exercice_started?.Invoke(exerciseData);
    }

    public virtual void OnNewExerciceStepStarted(ExerciceStep exerciceStep)
    {
        Exercice_step?.Invoke(exerciceStep);
    }


    #endregion

    #region Utensil spawning and utensil events (THIS MUST BE REWORKED)
    public virtual void OnNewUtensilSpawned(UtensilBehaviour newUtensil)
    {
        NewUtensilSpawned?.Invoke(newUtensil);
    }

    public virtual void OnUtensilObjectiveCompleted(UtensilBehaviour completedUtensil)
    {
        ObjectiveCompleted?.Invoke(completedUtensil);
    }

    public virtual void OnUtensilObjectiveFailed(UtensilBehaviour failedUtensil)
    {
        ObjectiveFailed?.Invoke(failedUtensil);
    }


    #endregion

    #region reworked utensil stuff for extensibility

    public virtual void OnNewItemSpawned(GameObject newItem)
    {
        NewItemSpawned?.Invoke(newItem);
    }

    public virtual void OnObjectiveCompleted(int index, GameObject declarer)
    {
        ObjectiveCompletedU?.Invoke(index, declarer);
    }

    public virtual void OnObjectiveFailed(int index, GameObject declarer)
    {
        ObjectiveFailedU?.Invoke(index, declarer);
    }

    public virtual void OnUtensilTriggerEnter(UtensilBehaviour triggeredUtensil, GameObject collider)
    {
        ObjectiveCompleted?.Invoke(triggeredUtensil);
    }

    public virtual void OnUtensilTriggerExit(UtensilBehaviour leftUtensil, GameObject exitedCollider)
    {
        ObjectiveFailed?.Invoke(leftUtensil);
    }

    #endregion

    #region Other related external to exercice events

    public virtual void OnMatlabResultsReceived() //does not pass any data. Called when matlab results have been received and correctly applied to all utensils.
    {
        MatlabResultsReceived?.Invoke();
    }

    public virtual void OnFinalMatlabResultsReceived() //does not pass any data. Called when matlab results have been received and correctly applied to all utensils.
    {
        PostExerciceMatlabResultsReceived?.Invoke();
    }

    public virtual void OnCalibrationDone(Vector3 position, Quaternion rotation) //does not pass any data. Called when matlab results have been received and correctly applied to all utensils.
    {
        calibrationDone?.Invoke(position, rotation);
    }
    #endregion

    #region Debug
    public virtual void OnLog(string logMessage) // called to log data, maybe in the future send it for display
    {
        LogDataUpdated?.Invoke(logMessage);
    }

    public virtual void OnReset() //called to reset the app to the starting state (no exercise)
    {
        ResetApp?.Invoke();
    }
    #endregion


    
}
