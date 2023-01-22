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


    public struct PerformanceSummary
    {
        public Dictionary<string, string> metricToValueList; //contains metrics used and their value
        public int totalPerformancePourcent;
    }


    [SerializeField] private ClockHandler exercice_clock; //Instance to object that keeps track of the exercice time, the time can be used for results calculation
    [SerializeField] private GameObject performance_hideable; //to hide results until all score come in
    [Header("single performances")]
    [SerializeField] private TextMesh performance_name_text;
    [SerializeField] private TextMesh performance_value_text;

    [Header("Total performance")]
    [SerializeField] private MeshRenderer[] starMeshes; //stars based on the performance
    [SerializeField] private TextMesh performance_total_text;

    [Header("Materials")]
    [SerializeField] private Material greyStarMaterial;
    [SerializeField] private Material yellowStarMaterial;


    private List<GameObject> registeredSpawnedItems = new List<GameObject>();

    private IScoreCalculator exerciseScoreCalculator;

    private void Awake()
    {
        performance_name_text.text = "Waiting for exercice to start";
        performance_hideable.SetActive(false);
        
    }


    private void OnEnable()
    {
        EventHandler.OnTutorialStarted += OnTutorialStarted;
        EventHandler.OnExerciseLoaded += OnExerciseLoaded;
        EventHandler.OnItemSpawned += OnItemSpawned;
        EventHandler.OnExerciseStarted += OnExerciseStarted;
        EventHandler.OnFinalMatlabDataReceived += CalculateAndDisplayResults; //we only calculate data after the final matlab results have come
        EventHandler.OnAppReset += ResetScoreDisplay;
    }

    private void OnDisable()
    {
        EventHandler.OnTutorialStarted -= OnTutorialStarted;
        EventHandler.OnExerciseLoaded -= OnExerciseLoaded;
        EventHandler.OnItemSpawned -= OnItemSpawned;
        EventHandler.OnExerciseStarted -= OnExerciseStarted;
        EventHandler.OnFinalMatlabDataReceived -= CalculateAndDisplayResults; //we only calculate data after the final matlab results have come
        EventHandler.OnAppReset -= ResetScoreDisplay;
    }

    private void OnTutorialStarted(TutorialData data) {performance_hideable.SetActive(false); performance_name_text.text = "Tutorial in progress";}//we simply change the text displayed

    private void OnExerciseLoaded(FullExerciceData exercise) => exerciseScoreCalculator = exercise.scoreCalculator; //we updated the calculator to the one related to the exercise

    private void OnItemSpawned(GameObject item) => registeredSpawnedItems.Add(item); //we register spawned utensils

    private void OnExerciseStarted(ExerciceData exercice) => performance_name_text.text = "Exercice is in progress !";

    //called when the exercice is 
    private void CalculateAndDisplayResults()
    {
        ResetScoreDisplay();
        if (exerciseScoreCalculator == null)
        {
            performance_name_text.text = "No score to display";
            return;
        }


        PerformanceSummary summary = exerciseScoreCalculator.CalculateScore(registeredSpawnedItems, exercice_clock);

        //individual summary
        foreach (KeyValuePair<string, string> entry in summary.metricToValueList)
        {
            performance_name_text.text += entry.Key + "\n";
            performance_value_text.text += entry.Value + "\n";
        }
        //stars
        int totalStars = getTotalStars(summary.totalPerformancePourcent);
        for (int i = 0; i < totalStars; i++)
        {
            starMeshes[i].material = yellowStarMaterial;
        }

        //total
        performance_total_text.text = "" + summary.totalPerformancePourcent;
        performance_hideable.SetActive(true);

    }


    private void ResetScoreDisplay()
    {
        performance_name_text.text = "";
        performance_value_text.text = "";
        performance_total_text.text = "";
        //TODO : hide stars
        for (int i = 0; i < starMeshes.Length; i++) starMeshes[i].material = greyStarMaterial;
        
    }


    //returns the amount of stars displayed based on pourcent performance
    private int getTotalStars(int totalPourcent)
    {
        if (totalPourcent <= 40) return 0;
        else if (totalPourcent <= 70) return 1;
        else if (totalPourcent <= 95) return 2;
        else return 3;
    }

}
