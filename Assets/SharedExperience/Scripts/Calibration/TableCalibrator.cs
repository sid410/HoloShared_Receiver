using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//used for testing the border of the table
public class TableCalibrator : MonoBehaviour
{

    private void Start()
    {
        if (!LocalItemSpawner.SINGLE_HOLO_BEHAVIOUR) Destroy(gameObject); //for now we only enable this for debug vrsions.

        //object manipulator, to listen to changes to location
        ObjectManipulator objectManip = GetComponent<ObjectManipulator>();
        objectManip.OnManipulationStarted.AddListener(onManipulationStart);
        objectManip.OnManipulationEnded.AddListener(onManipulationEnd);
        EventHandler.Instance.LogMessage("Calibrator " + gameObject.name + " spawned ! ");

    }



    private void OnCollisionEnter(Collision collision)
    {
        //itemSpawner.UpdatelogTexts("Bottle entered collision");
    }

    private void OnCollisionExit(Collision col)
    {
        //itemSpawner.UpdatelogTexts("Bottle Left collision");
    }

    private void onManipulationStart(ManipulationEventData eventData)
    {
        EventHandler.Instance.LogMessage("Start manipulating the boundary : " + gameObject.name);
    }

    private void onManipulationEnd(ManipulationEventData eventData)
    {
        EventHandler.Instance.LogMessage("Finished manipulating the boundary : " + gameObject.name 
            + " , New location is (" + gameObject.transform.position.x + "," + gameObject.transform.position.y + ")");
    }
}
