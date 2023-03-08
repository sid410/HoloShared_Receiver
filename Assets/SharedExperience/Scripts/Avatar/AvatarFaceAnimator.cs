using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarFaceAnimator : MonoBehaviour
{

    //UNUSED, CHANGED AVATAR WITH CUTE BUNNY
    public enum FaceExpression
    {
        DEFAULT, SMILE, CONFUSED, ANGRY, DISTRACTED, SURPRISED
    }

    [System.Serializable]
    public class AnimationData
    {
        public FaceExpression expression;
        public AnimationClip animation;
    }

    private Animator animator;
    [SerializeField] private List<AnimationData> faceAnimationList = new List<AnimationData>();

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetLayerWeight(1, 1);
    }

    private void OnEnable()
    {
        EventHandler.OnTutorialStepStarted += OnNewTutorialStep;
        EventHandler.OnExerciseStarted += OnExercise;
    }

    private void OnDisable()
    {
        EventHandler.OnTutorialStepStarted -= OnNewTutorialStep;
        EventHandler.OnExerciseStarted -= OnExercise;
    }

    private void OnNewTutorialStep(TutorialData.TutorialStep ts) => ChangeFace(ts.faceExpression);

    private void OnExercise(ExerciceData ed) => ChangeFace(FaceExpression.DEFAULT); //for now default face for exercise
    //changes the displayed face
    void ChangeFace(FaceExpression expression)
    {
        AnimationClip animation = faceAnimationList.Find((ad) => ad.expression == expression).animation;
        animator.CrossFadeInFixedTime(animation.name, 0);
    }
}
