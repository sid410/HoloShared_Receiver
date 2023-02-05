using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//used for the tutorial, the laser has a way smaller range for testing purposes
public class TutorialMazeLaserShot : MazeLazerAbs
{

    [SerializeField] MazeLaser laserInstance;
    [SerializeField]  Material laserMaterial;

    void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateLaser(0.2f);
    }
    // Update is called once per frame
    void OnDisable()
    {
        if (laserInstance != null) Destroy(laserInstance.Cleanup());
    }

    public override void UpdateLaser(float laserWidth)
    {
        if (laserInstance != null) Destroy(laserInstance.Cleanup());
        laserInstance = new MazeLaser(gameObject.transform.position, -gameObject.transform.up, laserMaterial, laserWidth);
    }
}
