using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarFaceAnimator : MonoBehaviour
{

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
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.Tutorial_step += OnNewTutorialStep;
    }

    private void OnDisable()
    {
        EventHandler.Tutorial_step -= OnNewTutorialStep;
    }

    private void OnNewTutorialStep(TutorialData.TutorialStep ts) => ChangeFace(ts.faceExpression);
    //changes the displayed face
    void ChangeFace(FaceExpression expression)
    {
        string animationName = faceAnimationList.Find((ad) => ad.expression == expression).animation.name;
        animator.CrossFade(animationName, 0);
    }
}
