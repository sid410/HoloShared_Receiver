using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//handles spawning and the behaviour of the beam
public class MazeLaserShot : MonoBehaviour
{
    //public Transform beamSpawnPosition;
    MazeLaser laserInstance;
    public Material laserMaterial;
    private bool exerciseStarted = true;

    private void OnEnable()
    {
        EventHandler.OnExerciseStepOver += EndLaser;
        EventHandler.OnAfterMatlabDataReceived += UpdateLaser;
        UpdateLaser();
    }

    private void OnDisable()
    {
        EventHandler.OnExerciseStepOver -= EndLaser;
        EventHandler.OnAfterMatlabDataReceived -= UpdateLaser;
        if (laserInstance != null) Destroy(laserInstance.Cleanup()); //we destroy the laser if this is ever disabled
    }

    private void EndLaser(){exerciseStarted = false;}


    private void Update()
    {
        UpdateLaser(); //remove after
    }

    //update the laser 
    private void UpdateLaser()
    {
        if (!exerciseStarted) return;
        if (laserInstance != null) Destroy(laserInstance.Cleanup());
        laserInstance = new MazeLaser(gameObject.transform.position, -gameObject.transform.up, laserMaterial);

    }
   
}
