using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles animation events for the tutorial maze animation
public class MazeTutorialEventHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject mirrorObject; 
    [SerializeField] private MazeLazerAbs laserHandler;
    [SerializeField] private GameObject avatarPosition; //avatar plays a part in the animation

    

    private void Start()
    {

        if (avatarPosition == null) return;
        EventHandler.Instance.InformNewAvatarDestination(avatarPosition);
        EventHandler.OnAvatarReachedDestination += StartAnimation;
    }

    private void OnDisable()
    {
        EventHandler.OnAvatarReachedDestination -= StartAnimation;
    }

    //the animation is started when the Avatar is in position

    private void StartAnimation()
    {
        animator.SetTrigger("start"); //we trigger the animation
    }
    public void UpdateLaser(float laserLength)
    {
        if (laserHandler != null) laserHandler.UpdateLaser(laserLength);
    }

    public void DisplayMirror(int isShown)
    {
        mirrorObject.SetActive(isShown == 1);
    }
}
