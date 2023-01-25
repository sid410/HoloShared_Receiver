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

    [Header("Dependencies")]
    [SerializeField] private ClockHandler exercice_clock; //Instance to object that keeps track of the exercice time, the time can be used for results calculation
    [SerializeField] private GameObject performance_hideable; //to hide results until all score come in

    [Header("Parts of the UI")]
    [SerializeField] private GameObject total_part;
    [SerializeField] private GameObject Stars_part;

    [Header("Total performance")]
    [SerializeField] private MeshRenderer[] starMeshes; //stars based on the performance
    [SerializeField] private TextMesh performance_total_text;

    [Header("Prefabs")]
    [SerializeField] private PerformanceEntry performance_metric_line_prefab; //for a line of performance.

    [Header("Materials")]
    [SerializeField] private Material greyStarMaterial;
    [SerializeField] private Material yellowStarMaterial;
    [SerializeField] private Color worstTotalColor;
    [SerializeField] private Color bestTotalColor;

    [Header("numbers for positioning")]
    [SerializeField] Vector3 perfLine_initialPos = new Vector3(-0.141f, 0.075f, 0); //initial position of the first performance line
    [SerializeField] float perfLine_gap = -0.03f; //gap between performance entries

    [Header("debug")]
    [SerializeField] private IScoreCalculator testScoreCalculator; //used for testing
    //spawned object trackers
    private List<GameObject> registeredSpawnedItems = new List<GameObject>();
    private Queue<GameObject> spawnedPerfLineQueue = new Queue<GameObject>();

    //gradient for score color based on performance
    private Gradient gradient;

    //exercise related calculator
    private IScoreCalculator exerciseScoreCalculator;

    private void Awake()
    {
        ResetScoreDisplay();
        //performance_hideable.SetActive(false);
        gradient = new Gradient();
        //we prepare the gradients
        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        GradientColorKey[]  colorKey = new GradientColorKey[2];
        colorKey[0].color = worstTotalColor;
        colorKey[0].time = 0.0f;
        colorKey[1].color = bestTotalColor;
        colorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);

    }


    private void OnEnable()
    {
        //EventHandler.OnTutorialStarted += OnTutorialStarted;
        EventHandler.OnExerciseLoaded += OnExerciseLoaded;
        EventHandler.OnItemSpawned += OnItemSpawned;
        //EventHandler.OnExerciseStarted += OnExerciseStarted;
        EventHandler.OnFinalMatlabDataReceived += CalculateAndDisplayResults; //we only calculate data after the final matlab results have come
        EventHandler.OnAppReset += ResetScoreDisplay;
    }

    private void OnDisable()
    {
        //EventHandler.OnTutorialStarted -= OnTutorialStarted;
        EventHandler.OnExerciseLoaded -= OnExerciseLoaded;
        EventHandler.OnItemSpawned -= OnItemSpawned;
        //EventHandler.OnExerciseStarted -= OnExerciseStarted;
        EventHandler.OnFinalMatlabDataReceived -= CalculateAndDisplayResults; //we only calculate data after the final matlab results have come
        EventHandler.OnAppReset -= ResetScoreDisplay;
    }

    //private void OnTutorialStarted(TutorialData data) {performance_hideable.SetActive(false); performance_name_text.text = "Tutorial in progress";}//we simply change the text displayed

    private void OnExerciseLoaded(FullExerciceData exercise)
    {
        if (exerciseScoreCalculator != null) exerciseScoreCalculator.Cleanup(); //we cleanup the script in case it is needed
        exerciseScoreCalculator = exercise.scoreCalculator; //we updated the calculator to the one related to the exercise
    }

    private void OnItemSpawned(GameObject item) => registeredSpawnedItems.Add(item); //we register spawned utensils

    //private void OnExerciseStarted(ExerciceData exercice) => performance_name_text.text = "Exercice is in progress !";

    //called when the exercice is 
    private void CalculateAndDisplayResults()
    {
        ResetScoreDisplay();
        if (exerciseScoreCalculator == null) return;

        //performance_hideable.SetActive(true);
        PerformanceSummary summary = exerciseScoreCalculator.CalculateScore(registeredSpawnedItems, exercice_clock);

        //individual summary
        int numEntries = 0;
        foreach (KeyValuePair<string, string> entry in summary.metricToValueList)
        {
            PerformanceEntry performanceEntry = Instantiate(performance_metric_line_prefab, this.gameObject.transform);
            performanceEntry.transform.localPosition = perfLine_initialPos + new Vector3(0, perfLine_gap * numEntries++, 0); //we instantiate an entry line for every performance statistic
            performanceEntry.UpdateUI(entry.Key, entry.Value);
            spawnedPerfLineQueue.Enqueue(performanceEntry.gameObject); //we save the spawned lines to delete later
        }

        AnimateTotal(summary.totalPerformancePourcent);

    }


    private void AnimateTotal(int totalValue)
    {
        performance_total_text.text = "0"; //we first set to 0%
        total_part.SetActive(true);
        Stars_part.SetActive(true);
        StartCoroutine(AnimateTotalIncremental(totalValue, performance_total_text, gradient));
    }

    //this is for the total, also enables stars
    IEnumerator AnimateTotalIncremental(int totalValue, TextMesh animatedText, Gradient colorGradient)
    {
        int counter = 0;
        while (counter < totalValue)
        {
            counter++;
            animatedText.text = "" + counter;
            if (colorGradient != null)
            {
                Color textColor = colorGradient.Evaluate((float)counter / 100f); //we set the color based on the value
                animatedText.color = textColor;

                //we enable a star depending on the stade we reached in the animation
                int star = getYellowedStarByPercent(counter);
                if (star != -1) starMeshes[star].material = yellowStarMaterial;
            }

            yield return new WaitForSeconds(0.02f);
        }

    }


    private void ResetScoreDisplay()
    {
        Stars_part.SetActive(false);
        total_part.SetActive(false);
        performance_total_text.text = "";
        //we delete all spawned performance metric
        while(spawnedPerfLineQueue.Count > 0)
        {
            GameObject perfLine = spawnedPerfLineQueue.Dequeue();
            Destroy(perfLine.gameObject);
        }
        //we reset the stars material to grey
        for (int i = 0; i < starMeshes.Length; i++) starMeshes[i].material = greyStarMaterial;
        
    }

    //returns the index of the star to set to yellow depending on the passed pourcent, used for animated stars display with the text increasing
    private int getYellowedStarByPercent(int pourcent)
    {
        switch (pourcent)
        {
            case 40: return 0;
            case 70: return 1;
            case 95: return 2;
            default: return -1;
        }
    }

    //returns the amount of stars displayed based on pourcent performance
    private int getTotalStars(int totalPourcent)
    {
        if (totalPourcent <= 40) return 0;
        else if (totalPourcent <= 70) return 1;
        else if (totalPourcent <= 95) return 2;
        else return 3;
    }


    //debug functions. Uses a debug score calculator to test the interface
    public void DebugScoring()
    {
        exerciseScoreCalculator = testScoreCalculator;
        CalculateAndDisplayResults();
    }

}
