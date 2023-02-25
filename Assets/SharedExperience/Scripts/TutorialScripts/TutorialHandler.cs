using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TutorialData;

/**
 * Dispatches the various tutorial step events to the releveant listeners.
 * 
 */
public class TutorialHandler : MonoBehaviour
{

    //Current Tutorial information. This is used to handle transitionning steps from steps in the tutorial and inform all the parts of the exercice
    private int currentStepIndex = -1;
    private float currentStepDuration = 0f;
    private float countdown = 0f;

    private TutorialData currentTutorial = null;

    // Start is called before the first frame update
    void Start()
    {
        currentStepIndex = -1;
        
    }

    #region subscriptions
    private void OnEnable()
    {
        EventHandler.OnTutorialStarted += InitTutorial;
        EventHandler.OnAppReset += OnReset;
    }

    private void OnDisable()
    {
        EventHandler.OnTutorialStarted -= InitTutorial;
        EventHandler.OnAppReset -= OnReset;
    }

    #endregion

    private void Update()
    {
        if (currentStepIndex == -1 || countdown <= 0f) return; //we ignore if no tutorial

        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            ++currentStepIndex; //we increment the step index;
            if (currentStepIndex >= currentTutorial.steps.Count) EndTutorial(); //if the tutorial finished, we end it
            else InitTutorialStep(currentTutorial.steps[currentStepIndex]); //we move into the next tutorial step
        }

    }

    //called when a full exercice data has been selected, we initialize the tutorial
    private void InitTutorial(TutorialData tutorial)
    {
        if (tutorial == null) return; //maybe trigger exercice start here if no tutorial
        countdown = 0f;
        currentStepIndex = 0;
        currentTutorial = tutorial;
        InitTutorialStep(currentTutorial.steps[0]);
    }


    //Called between each step using the countdown to move from one step to another
    private void InitTutorialStep(TutorialStep tutorialStep)
    {
        if (currentStepIndex == -1) return;
        currentStepDuration = tutorialStep.duration > 0 ? tutorialStep.duration : Util.CalculateMessageDisplayTime(tutorialStep.avatarText.message); // we calculate the duration of this tutorial step.
        EventHandler.Instance.StartTutorialStep(tutorialStep); //we inform all other listeners about the new step.
        countdown = currentStepDuration; //we start the countdown
    }

    private void EndTutorial()
    {
        if (currentStepIndex == -1) return;
        currentStepIndex = -1;
        currentTutorial = null;
        EventHandler.Instance.EndTutorial();
    }

    //called when the app is reset
    private void OnReset()
    {
        currentStepIndex = -1;
    }
}
