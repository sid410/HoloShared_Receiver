using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AvatarFaceAnimator;



[CreateAssetMenu(fileName = "Assets/Resources/Data/TutorialData/tutorial.asset", menuName = "ScriptableObjects/Create new tutorial", order = 3)]
public class TutorialData : ScriptableObject
{
    /* THINKING :
     * - Should be a list of Steps 
     * - Step = Avatar Animation, Text (3x34 letters max), Spawnables (Location ? Maybe just put Independant scripts inside the prefabs, so have a distinct prefab), maybe highlights, duration
     */
    
    [System.Serializable]
    public class TutorialStep
    {
        [Tooltip("If 0, calculate dynamically the duration of this step (tutorialHandler)")]
        public float duration; //duration of the tutorial

        //avatar data
        public FaceExpression faceExpression;

        [TextArea(3, 1)]
        public string message;
        //TODO : maybe also body animation

        public ItemsSpawnEntry tutorialitems;
    }

    public List<TutorialStep> steps = new List<TutorialStep>();

    
}
