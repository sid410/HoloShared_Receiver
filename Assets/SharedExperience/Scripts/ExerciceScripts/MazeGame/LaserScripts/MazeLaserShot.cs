using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//handles spawning and the behaviour of the beam
public class MazeLaserShot : MazeLazerAbs
{
    //public Transform beamSpawnPosition;
    MazeLaser laserInstance;
    public Material laserMaterial;

    [Tooltip("recast the laser on update, this is for debugging purposes. With the exercise one, the laser update is called every time results are received")]
    public bool debug_recastOnUpdate = false;
    private bool exerciseStarted = true;

    private void OnEnable()
    {
        EventHandler.OnExerciseStepOver += EndLaser;
        EventHandler.OnAfterMatlabDataReceived += RecastLaser;
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateLaser(5f);
    }

    private void OnDisable()
    {
        EventHandler.OnExerciseStepOver -= EndLaser;
        EventHandler.OnAfterMatlabDataReceived -= RecastLaser;
        if (laserInstance != null) Destroy(laserInstance.Cleanup()); //we destroy the laser if this is ever disabled
    }

    private void EndLaser(){exerciseStarted = false;}


     private void Update()
     {
        if (!debug_recastOnUpdate) return;
        UpdateLaser(5f); //remove after
     }

    //update the laser 

    private void RecastLaser() => StartCoroutine(RecastLaserAfterDelay());

    IEnumerator RecastLaserAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateLaser(5f);
    }
    public override void UpdateLaser(float laserWidth)
    {
        if (!exerciseStarted) return;
        if (laserInstance != null) Destroy(laserInstance.Cleanup());
        laserInstance = new MazeLaser(gameObject.transform.position, -gameObject.transform.up, laserMaterial);

    }
   
}
