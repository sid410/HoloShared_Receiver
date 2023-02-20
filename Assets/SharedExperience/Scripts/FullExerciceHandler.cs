using extOSC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private ExerciseDifficulty currentDifficulty = ExerciseDifficulty.NORMAL;
    private ExerciceData startedExercise = null; //the exercise that was started based on diffculty

    private int currentExerciseStepIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        Receiver = GameObject.FindObjectOfType<OSCReceiver>();
        Receiver.Bind("/loadexercise", LoadExercice); //we bind to the receiver, this will trigger the exercise from the phone app eventually
        Receiver.Bind("/resetapp", TriggerAppReset);
    }

    #region subscriptions
    private void OnEnable()
    {
        EventHandler.OnTutorialOver += StartExercise;
        EventHandler.OnFinalMatlabDataProcessed += GoToNextExerciseStep; //the order is stepOver => Kinect results received => New step (so we can calculate scores effectively)

    }

    private void OnDisable()
    {
        EventHandler.OnTutorialOver -= StartExercise;
        EventHandler.OnFinalMatlabDataProcessed -= GoToNextExerciseStep;
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
        ExerciseType exerciseType = ExerciseType.MAZE; //if we receive no message, we start the simple 
        currentDifficulty = ExerciseDifficulty.NORMAL;
        startedExercise = null;

        //we get the ee
        if (message != null && message.Values.Count > 0)
        {
            //we get the exercise
            int exerciseTypeInt = message.Values[0].IntValue;
            exerciseType = (ExerciseType)exerciseTypeInt;

            //if difficulty is also specified in the message, we use it.
            if (message.Values.Count > 1)
            {
                int difficultyInt = message.Values[1].IntValue;
                currentDifficulty = (ExerciseDifficulty)difficultyInt;
            }
        } 
        
        ExercisePreset exercisePreset = loadedExercises.Find((ex) => ex.exerciseEnum == exerciseType); //we get the associated exercise data
        currentExercise = exercisePreset.exerciseDataObject;
        EventHandler.Instance.LoadExercise(currentExercise); //we inform all listeners of the loaded exercise

        //if we skip the tutorial, we start the exercise.
        if (!SkipTutorial) StartTutorial();
        else StartExercise();
    }


    //
    void StartExercise()
    {
        //we announce the start of the exercise phase
        startedExercise = currentExercise.exercisesByDifficulty.First((ex) => ex.exerciseDifficulty == currentDifficulty).exercise; //we find the exercise depending on the 
        if (startedExercise == null) startedExercise = currentExercise.exercisesByDifficulty.First().exercise; //if the difficulty is null (didn't have the difficulty needed) , we get the fiirst from the list for debugging 
        EventHandler.Instance.StartExercise(startedExercise);

        //we start the first step
        currentExerciseStepIndex = 0;
        EventHandler.Instance.StartExerciseStep(startedExercise.steps[currentExerciseStepIndex]); //TODO : fix the distinction between steps and exercise to be less convuluted, also multiple steps 
    }

    //we go to the next exercise Step
    void GoToNextExerciseStep()
    {
        if (currentExercise == null) return;
        currentExerciseStepIndex++;
        if (currentExerciseStepIndex >= startedExercise.steps.Count) EventHandler.Instance.EndExercise();
        else EventHandler.Instance.StartExerciseStep(startedExercise.steps[currentExerciseStepIndex]);
    }
    void StartTutorial()
    {
        if (currentExercise.tutorial == null || currentExercise.tutorial.steps.Count == 0)
        {
            StartExercise();
            return;
        }
        EventHandler.Instance.StartTutorial(currentExercise.tutorial);
    }

    //Resets the app to the state it is at the start (no exercise loaded)
    public void TriggerAppReset(OSCMessage message)
    {
        EventHandler.Instance.ResetApp();
    }
}
