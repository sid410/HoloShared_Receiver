using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//handles spawning and the behaviour of the beam
public class MazeLaserShot : MonoBehaviour
{
    //public Transform beamSpawnPosition;
    MazeLaser laserInstance;
    public Material laserMaterial;
    private bool exerciseStarted = false;

    private void OnEnable()
    {
        EventHandler.OnExerciseStarted += StartLaser;
        EventHandler.OnExerciseOver += EndLaser;
        EventHandler.OnMatlabDataReceived += UpdateLaser;
    }

    private void OnDisable()
    {
        EventHandler.OnExerciseStarted -= StartLaser;
        EventHandler.OnExerciseOver -= EndLaser;
        EventHandler.OnMatlabDataReceived -= UpdateLaser;
        if (laserInstance != null) Destroy(laserInstance.getSpawnedObj()); //we destroy the laser if this is ever disabled
    }


    private void StartLaser(ExerciceData ed) => exerciseStarted = true;

    private void EndLaser() => exerciseStarted = false;
    //update the laser 
    private void UpdateLaser()
    {
        if (!exerciseStarted) return;
        if (laserInstance != null) Destroy(laserInstance.getSpawnedObj());
        laserInstance = new MazeLaser(gameObject.transform.position, -gameObject.transform.up, laserMaterial);

    }
   
}
