using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//used for the tutorial, the laser has a way smaller range for testing purposes
public class TutorialMazeLaserShot : MonoBehaviour
{

    [SerializeField] MazeLaser laserInstance;
    [SerializeField]  Material laserMaterial;

    void Start()
    {
        laserInstance = new MazeLaser(gameObject.transform.position, -gameObject.transform.up, laserMaterial, 0.2f);
    }

    // Update is called once per frame
    void OnDisable()
    {
        if (laserInstance != null) Destroy(laserInstance.Cleanup());
    }
}
