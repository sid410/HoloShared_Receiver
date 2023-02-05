using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles animation events for the tutorial maze animation
public class MazeTutorialEventHandler : MonoBehaviour
{

    [SerializeField] private GameObject mirrorObject;

    // Update is called once per frame
    [SerializeField] private MazeLazerAbs laserHandler;
    //called by the animation event
    public void UpdateLaser(float laserLength)
    {
        if (laserHandler != null) laserHandler.UpdateLaser(laserLength);
    }

    public void DisplayMirror(int isShown)
    {
        mirrorObject.SetActive(isShown == 1);
    }
}
