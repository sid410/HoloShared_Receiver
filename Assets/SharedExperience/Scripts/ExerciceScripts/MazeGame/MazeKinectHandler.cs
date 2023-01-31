using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//handles kinect data for the maze exercise. Old items are destroyed and new prefabs are created
[CreateAssetMenu(fileName = "Assets/Resources/Data/KinectHandlers/MazeKinectH.asset", menuName = "ScriptableObjects/Kinect Handlers/Maze Kinect Handler", order = 2)]
public class MazeKinectHandler : KinectResultsAbs
{
    [Header("prefabs")]
    public GameObject utensilMirrorPrefab;
    public GameObject utensilRefactorPrefab;

    private GameObject StoneOrigin;
    Queue<GameObject> spawnedMirrors = new Queue<GameObject>();

    private bool blockKinectResults = false;

    public override void Init()
    {
        StoneOrigin = GameObject.Find("StonesOrigin");
        EventHandler.OnExerciseStepOver += OnExerciseStepOver;
        EventHandler.OnExerciseStepStarted += OnExerciseStepStarted; 
    }

    //setters for subscriptions
    private void OnExerciseStepStarted(ExerciceData.ExerciceStep es) => SetBlockKinectResults(false);

    private void OnExerciseStepOver() => SetBlockKinectResults(true); //we don't want the final kinect results to affect our items, we want the change of items to stop when the objectives are reached
    private void SetBlockKinectResults(bool newVal) => blockKinectResults = newVal;



    public override void HandleKinectData(List<localKinectReceiver.KinectUtensilData> kinectResults)
    {
        if (blockKinectResults) return;

        DestroyOldMirrors(); //destroy old mirrors

        //we spawn new mirrors.
        foreach (localKinectReceiver.KinectUtensilData kr in kinectResults)
        {
            if (kr.type == UtensilExtension.UtensilType.UNDETECTED) continue;
            GameObject spawnedItem;
            float rotation;

            //we get the relevant data from the type of object
            switch (kr.type)
            {
                case UtensilExtension.UtensilType.CUP:
                case UtensilExtension.UtensilType.BOTTLE:
                    spawnedItem = Instantiate(utensilRefactorPrefab);
                    spawnedItem.transform.parent = StoneOrigin.transform;
                    rotation = 0;
                    break;
                default:
                    spawnedItem = Instantiate(utensilMirrorPrefab);
                    spawnedItem.transform.parent = StoneOrigin.transform;
                    rotation = kr.orientation;
                    break;
            }

            //we save the object
            UtensilAbs utensilBehaviour = spawnedItem.GetComponent<UtensilAbs>();
            utensilBehaviour.RepositionRealGameobject(kr.position, rotation);
            spawnedMirrors.Enqueue(spawnedItem);
        }
    }


    private void DestroyOldMirrors()
    {
        if (spawnedMirrors == null || spawnedMirrors.Count == 0) return;

        while (spawnedMirrors.Count > 0)
        {
            GameObject mirrorObj = spawnedMirrors.Dequeue();
            Destroy(mirrorObj.gameObject);
        }
    }

    public override void Cleanup()
    {
        EventHandler.OnExerciseStepOver -= OnExerciseStepOver;
        EventHandler.OnExerciseStepStarted -= OnExerciseStepStarted;
    }
}
