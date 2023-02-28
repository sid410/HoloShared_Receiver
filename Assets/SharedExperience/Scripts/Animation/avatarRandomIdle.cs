using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class avatarRandomIdle : StateMachineBehaviour
{


    //random idle animatinos that can be triggered at any time
    string[] randomTriggers = { "ra_turn", "ra_hover" };


    [Header("Cooldown after the random silly animations are played. if on cooldown the basic idle animation is palyed")]
    [SerializeField] private float animationCooldown = 5f;

    //state hash
    int defaultStateHash = Animator.StringToHash("avatar_idle");
    //random animation vars
    private float currentCooldown;


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(" (SU) State is " + stateInfo.fullPathHash);
        if (animator.GetBool("Moving")) return; //if we're moving, ignore any special animation

        //we calculate the cooldown
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
            return;
        }
        currentCooldown = animationCooldown;

        int randomAnimationIndex = Random.Range(0, randomTriggers.Length); //we roll a random animation
        string randomAnim = randomTriggers[randomAnimationIndex];

        animator.SetTrigger(randomAnim);

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentCooldown = animationCooldown; //we reset the cooldown
    }
}
