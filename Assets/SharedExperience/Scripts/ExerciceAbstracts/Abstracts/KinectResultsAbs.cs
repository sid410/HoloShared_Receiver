using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//Impelmented by all exercises, receives a data pack of kinect items and their type, positions etc and uses them as see fit
public abstract class KinectResultsAbs : ScriptableObject
{
    
    public abstract void Init(); //called when a new exercise happens (called by the localKinectReceiverScript)


    //gets all kinect locations and uses it for the exercise
    public abstract void HandleKinectData(List<localKinectReceiver.KinectUtensilData> kinectResults);

    //called before removing the impelmentation 
    public abstract void Cleanup();
}
