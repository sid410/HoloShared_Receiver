using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//Impelmented by all exercises, receives a data pack of kinect items and their type, positions etc and uses them as see fit
public abstract class KinectResultsAbs : ScriptableObject
{
    
    public abstract void Init(); //called when a new exercise happens (called by the localKinectReceiverScript)

    //TODO : disable by unregestering
    public abstract void HandleKinectData(List<localKinectReceiver.KinectUtensilData> kinectResults);
}
