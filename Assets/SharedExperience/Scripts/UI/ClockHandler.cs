using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * Handles tracking the time that an exercice takes
 */
public class ClockHandler : MonoBehaviour
{

    [SerializeField] private TextMesh time_display;

    private bool exercice_in_progress = false;
    public float exercice_timer { private set; get; }

    // Start is called before the first frame update
    void Awake()
    {
        exercice_in_progress = false;
    }

    #region subscriptions
    private void OnEnable()
    {
        EventHandler.OnExerciseStarted += StartClock;
        EventHandler.OnExerciseOver += EndClock;
    }

    private void OnDisable()
    {
        EventHandler.OnExerciseStarted -= StartClock;
        EventHandler.OnExerciseOver -= EndClock;
    }
    #endregion
    private void Update()
    {
        if (!exercice_in_progress) return;
        exercice_timer += Time.deltaTime;
        UpdateDisplay();
    }

    void StartClock(ExerciceData exercice)
    {
        exercice_timer = 0f;
        exercice_in_progress = true;
    }

    void EndClock()
    {
        exercice_in_progress = false;
    }

    void UpdateDisplay()
    {
        time_display.text = getCurrentTimeString();
    }

    public String getCurrentTimeString()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(exercice_timer);
        return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }
}
