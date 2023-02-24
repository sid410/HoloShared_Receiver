using System;
using UnityEngine;
using static ExerciceData;
using static TutorialData;

public class EventHandler : MonoBehaviour
{
    public static EventHandler Instance { get; private set; }

    public static event Action<Vector3, Quaternion> OnCalibrationDone; //called after the calibration using vuforia is done.

    public static event Action<int, GameObject> OnObjectiveStepAchieved; //called when a certain objective step in completed. Objectives can have 1 step or be multi-step.
    public static event Action<int, GameObject> OnObjectiveCompleted;
    public static event Action<int, GameObject> OnObjectiveFailed; //when objective is failed after being completed (item moved out
    public static event Action<GameObject> OnItemSpawned;

    //DATA LOADING STEP
    public static event Action<FullExerciceData> OnExerciseLoaded;
    //Tutorial Steps
    public static event Action OnTutorialOver;
    public static event Action<TutorialData> OnTutorialStarted;
    public static event Action<TutorialStep> OnTutorialStepStarted;

    //exercice steps
    
    public static event Action<ExerciceData> OnExerciseStarted;
    public static event Action<ExerciceStep> OnExerciseStepStarted;
    public static event Action OnExerciseStepOver;
    public static event Action OnExerciseOver;

    //avatar events 
    public static event Action OnAvatarReachedDestination;
    //kinect events
    public static event Action OnBeforeMatlabDataReceived; //called every time we get matlab resutls
    public static event Action OnAfterMatlabDataReceived; //called every time we get matlab resutls
    public static event Action OnFinalMatlabDataReceived; //this is called for matlab results that happen after an exercice is over. Which basically marks the official end of the exercice
    public static event Action OnFinalMatlabDataProcessed; //called when the exercise officially finished parsing all necessary data

    //score calculation Event
    public static event Action OnScoreIncreaseStarted;
    public static event Action OnScoreIncreaseEnded;
    public static event Action OnStarAcquired;

    //debug events
    public static event Action<string> OnLog;
    public static event Action OnAppReset;


    private void Awake()
    {
        if (Instance != null) return;
        Instance = this;
    }


    public virtual void LoadExercise(FullExerciceData loadedExercise) // called when an exercise is loaded, all different parts are passed to all scripts
    { 
        OnExerciseLoaded?.Invoke(loadedExercise);
    }

    #region Tutorial step events


    public virtual void EndTutorial() //called to inform that the current exercice is over.
    {
        OnTutorialOver?.Invoke();
    }

    public virtual void StartTutorial(TutorialData tutorialData) // called when an exercice is started
    {
        OnTutorialStarted?.Invoke(tutorialData);
    }

    public virtual void StartTutorialStep(TutorialStep tutorialStep)
    {
        OnTutorialStepStarted?.Invoke(tutorialStep);
    }

    #endregion
    #region Exercice step events
    
    public virtual void StartExercise(ExerciceData exerciseData)// ExerciceData exerciceData // called when an exercice is started
    {
        OnExerciseStarted?.Invoke(exerciseData);
    }

    public virtual void StartExerciseStep(ExerciceStep exerciceStep)
    {
        OnExerciseStepStarted?.Invoke(exerciceStep);
    }

    public virtual void EndExerciseStep()
    {
        OnExerciseStepOver?.Invoke();
    }

    public virtual void EndExercise() //called to inform that the current exercice is over.
    {
        OnExerciseOver?.Invoke();
    }


    #endregion
    #region avatar events
    public virtual void InformAvatarReachedDestination() // called when the avatar receives a new position and reaches it!
    {
        OnAvatarReachedDestination?.Invoke();
    }
    #endregion
    #region reworked utensil stuff for extensibility

    public virtual void SpawnItem(GameObject newItem)
    {
        OnItemSpawned?.Invoke(newItem);
    }

    public virtual void SetObjectiveStepAsAchieved(int index, GameObject declarer)
    {
        OnObjectiveStepAchieved?.Invoke(index, declarer);
    }

    
    public virtual void SetObjectiveAsComplete(int index, GameObject declarer)
    {
        OnObjectiveCompleted?.Invoke(index, declarer);
    }

    public virtual void SetObjectiveAsUncomplete(int index, GameObject declarer)
    {
        OnObjectiveFailed?.Invoke(index, declarer);
    }

    #endregion

    #region Other related external to exercice events

    public virtual void TriggerBeforeMatlabReceived() //called before matlab data is used (so before any object/ exercise kinect data handler uses the data)
    {
        OnBeforeMatlabDataReceived?.Invoke();
    }
    public virtual void TriggerAfterMatlabReceived() //does not pass any data. Called when matlab results have been received and correctly applied to all utensils.
    {
        OnAfterMatlabDataReceived?.Invoke();
    }

    public virtual void TriggerFinalMatlabReceived() //does not pass any data. Called when matlab results have been received and correctly applied to all utensils.
    {
        OnFinalMatlabDataReceived?.Invoke();
    }

    public virtual void TriggerFinalMatlabProcessed() //does not pass any data. Called when matlab results have been received and correctly applied to all utensils.
    {
        OnFinalMatlabDataProcessed?.Invoke();
    }
    
    public virtual void TriggerCalibrationEnded(Vector3 position, Quaternion rotation) //does not pass any data. Called when matlab results have been received and correctly applied to all utensils.
    {
        OnCalibrationDone?.Invoke(position, rotation);
    }
    #endregion

    #region score calculation
    public virtual void TriggerStarAcquired() //does not pass any data. Called when matlab results have been received and correctly applied to all utensils.
    {
        OnStarAcquired?.Invoke();
    }

    public virtual void StartScoreIncrease() //called with the start of the increase of score pourcentage in results
    {
        OnScoreIncreaseStarted?.Invoke();
    }

    public virtual void EndScoreIncrease()
    {
        OnScoreIncreaseEnded?.Invoke();
    }
    #endregion
    #region Debug
    public virtual void LogMessage(string logMessage) // called to log data, maybe in the future send it for display
    {
        OnLog?.Invoke(logMessage);
    }

    public virtual void ResetApp() //called to reset the app to the starting state (no exercise)
    {
        OnAppReset?.Invoke();
    }
    #endregion


    
}
