using extOSC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * Main class that should (in the future) receive data from therapist. Use it to Loadup the Exercise and triggers the domino effect of Tutorial => Exercise => Result
 * 
 */
public class FullExerciceHandler : MonoBehaviour
{
    #region consts
    private float Xcalibration = -0.02f;
    private float rotationCalibrationY = 30f;
    #endregion
    #region classes and enums

    [System.Serializable]
    public class ExercisePreset
    {
        public ExerciseType exerciseEnum;
        public FullExerciceData exerciseDataObject;
    }

    #endregion

    [Tooltip("Contains all the available exercise that can be started")]
    public List<ExercisePreset> loadedExercises = new List<ExercisePreset>();

    private OSCReceiver Receiver; //receives commands from outside app
    private bool SkipTutorial = false; //Should be able to set a skip for the tutorial if possible.

    private FullExerciceData currentExercise = null;

    // Start is called before the first frame update
    void Start()
    {
        Receiver = GameObject.FindObjectOfType<OSCReceiver>();
        Receiver.Bind("/loadexercise", LoadExercice); //we bind to the receiver, this will trigger the exercise from the phone app eventually
    }

    #region subscriptions
    private void OnEnable()
    {
        EventHandler.Tutorial_over += StartExercise;
    }

    private void OnDisable()
    {
        EventHandler.Tutorial_over -= StartExercise;
    }
    #endregion

    //debug function : starts locally the exercise
    public void DebugStartExercise()
    {
        LoadExercice(null); //we wdebug start the exercise with nothing
    }

    //Loads an exercise from the database using data received from the therapist
    void LoadExercice(OSCMessage message)
    {
        //TODO : load if tutorial skip or not
        //TODO : load which type of exercise it is
        ExerciseType exerciseType = ExerciseType.MAZE; //for debugging purposes we use the utensil exercise
        if (message != null)
        {
            int utensilEnumInt = message.Values[0].IntValue;
            exerciseType = (ExerciseType)utensilEnumInt;
        } 
        
        //for now debug hardcded
        
        
        ExercisePreset exercisePreset = loadedExercises.Find((ex) => ex.exerciseEnum == exerciseType); //we get the associated exercise data
        currentExercise = exercisePreset.exerciseDataObject;
        EventHandler.Instance.OnExerciseLoaded(currentExercise); //we inform all listeners of the loaded exercise
        //if we skip the tutorial, we start the exercise.
        if (!SkipTutorial) StartTutorial();
        else StartExercise();
    }


    //
    void StartExercise()
    {
        //TODO : start exercise after delay
        EventHandler.Instance.OnExerciceStarted(currentExercise.exercice);
        EventHandler.Instance.OnNewExerciceStepStarted(currentExercise.exercice.steps[0]); //TODO : fix the distinction between steps and exercise to be less convuluted, also multiple steps 
    }


    void StartTutorial()
    {
        if (currentExercise.tutorial == null || currentExercise.tutorial.steps.Count == 0)
        {
            StartExercise();
            return;
        }
        EventHandler.Instance.OnTutorialStarted(currentExercise.tutorial);
    }
}
