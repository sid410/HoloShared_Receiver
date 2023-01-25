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
        EventHandler.OnObjectiveCompleted += EndLaser;
        //EventHandler.OnMatlabDataReceived += UpdateLaser;
    }

    private void OnDisable()
    {
        EventHandler.OnObjectiveCompleted -= EndLaser;
        //EventHandler.OnMatlabDataReceived -= UpdateLaser;
        if (laserInstance != null) Destroy(laserInstance.getSpawnedObj()); //we destroy the laser if this is ever disabled
    }

    private void EndLaser(int code, GameObject caller){if (code == MazeObjectives.INIT_OBJECTIVE_INDEX) exerciseStarted = false;}
    //update the laser 
    private void Update()
    {
        if (!exerciseStarted) return;
        if (laserInstance != null) Destroy(laserInstance.getSpawnedObj());
        laserInstance = new MazeLaser(gameObject.transform.position, -gameObject.transform.up, laserMaterial);

    }
   
}
