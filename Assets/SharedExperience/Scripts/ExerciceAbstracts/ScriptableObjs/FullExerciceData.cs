using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * This is an Exercice Data Object : It should contain 
 * 
 * - A reference to a tutorial => 
 * - A reference to the exercice Steps =>
 * - A reference to the score Calculator =>
 * 
 */

[CreateAssetMenu(fileName = "Assets/Resources/Data/FullExercise/fexercise.asset", menuName = "ScriptableObjects/Create Full exercise", order = 2)]
public class FullExerciceData : ScriptableObject
{

    //exercises are ranked by difficulty. depending on picked difficulty, exercise is started
    [System.Serializable]
    public class ExerciseToDifficultyData
    {
        public ExerciseDifficulty exerciseDifficulty;
        public ExerciceData exercise;
    }

    public ExerciseType exerciseType;
    public TutorialData tutorial; //Explanation phase
    public List<ExerciseToDifficultyData> exercisesByDifficulty = new List<ExerciseToDifficultyData>(); //exercise phase
    public KinectResultsAbs kinectResultHandler; //handles kinect results and uses them for the exercise 
    public IScoreCalculator scoreCalculator; //calculating and displaying score


    //we reorder by difficulty
    private void OnValidate()
    {
        exercisesByDifficulty.Sort((ebd1, ebd2) => ebd1.exerciseDifficulty.CompareTo(ebd2.exerciseDifficulty));
    }
}
