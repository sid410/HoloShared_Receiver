using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AvatarFaceAnimator;


[System.Serializable]
[CreateAssetMenu(fileName = "Assets/Resources/Data/ExerciseData/exercice.asset", menuName = "ScriptableObjects/Create Exercice (the middle)", order = 2)]

public class ExerciceData : ScriptableObject
{

    /**
     * Exercice 
     * 
     */

    [System.Serializable]
    public class ExerciceStep
    {
        [Tooltip("If limit is -1, there is no time limit")]
        float timeLimit = -1;

        //avatar data
        public FaceExpression faceExpression;

        public AvatarMessage avatarText;

        public IObjectiveHandler objectives;

        //spawnables handle registering to the objective list.
        public ItemsSpawnEntry spawnEntry; //used to spawn items
    }

    public List<ExerciceStep> steps = new List<ExerciceStep>();
}
