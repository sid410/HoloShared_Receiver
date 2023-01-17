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
    public ExerciseType exerciseType;
    public TutorialData tutorial; //Explanation phase
    public ExerciceData exercice; //exercise phase
    public KinectResultsAbs kinectResultHandler; //handles kinect results and uses them for the exercise 
    public IScoreCalculator scoreCalculator; //calculating and displaying score
    
}
