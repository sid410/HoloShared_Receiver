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
        public List<Tuple<string, string>> metricToValueList; //contains metrics used and their value
        public Tuple<String, String> totalValue;
        public int stars;
        public char Rank;
    }
    
    [SerializeField] private ClockHandler exercice_clock; //Instance to object that keeps track of the exercice time, the time can be used for results calculation
    [SerializeField] private GameObject[] stars; //stars based on the performance
    [SerializeField] private TextMesh RankLetter; //Ranks (D,C,B,A,S)
    [SerializeField] private TextMesh performance_name_text;
    [SerializeField] private TextMesh performance_value_text;
    [SerializeField] private TextMesh performance_total_text;

    private List<GameObject> registeredSpawnedItems = new List<GameObject>();

    private IScoreCalculator exerciseScoreCalculator;

    private void Start()
    {
        performance_name_text.text = "Waiting for exercice to start";
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

    private void OnTutorialStarted(TutorialData data) => performance_name_text.text = "Tutorial in progress"; //we simply change the text displayed

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


        performance_name_text.text = exerciseScoreCalculator.CalculateScore(registeredSpawnedItems, exercice_clock);

        PerformanceSummary summary  = new PerformanceSummary();
        foreach (Tuple<string, string> entry in summary.metricToValueList)
        {
            performance_name_text.text += (entry.Item1 + '\n');
            performance_value_text.text += (entry.Item2 + '\n');
        }

        performance_total_text.text = summary.totalValue.Item1 + ":" + summary.totalValue.Item2;
        //TODO : stars
        RankLetter.text = "" + summary.Rank;
        for (int i = 0; i < (( summary.stars > stars.Length) ? stars.Length : summary.stars); i++)
        {
            stars[i].SetActive(true);
        }

    }


    private void ResetScoreDisplay()
    {
        performance_name_text.text = "Waiting for a new exercise !";
        performance_value_text.text = "";
        RankLetter.text = "";
        performance_total_text.text = "";
        //TODO : hide stars
        for (int i = 0; i < stars.Length; i++) stars[i].SetActive(false);
        
    }

    //TODO : Create an interface thaat implements the Result calculation, so that calculation can be independant and adapted depending on the exercice
}
