using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//handles kinect data for the maze exercise. Old items are destroyed and new prefabs are created
[CreateAssetMenu(fileName = "Assets/Resources/Data/KinectHandlers/MazeKinectH.asset", menuName = "ScriptableObjects/Kinect Handlers/Maze Kinect Handler", order = 2)]
public class MazeKinectHandler : KinectResultsAbs
{
    public GameObject utensilMirrorPrefab;
    private GameObject StoneOrigin;
    Queue<GameObject> spawnedMirrors = new Queue<GameObject>();
    public override void Init()
    {
        StoneOrigin = GameObject.Find("StonesOrigin");
    }

    public override void HandleKinectData(List<localKinectReceiver.KinectUtensilData> kinectResults)
    {
        DestroyOldMirrors(); //destroy old mirrors

        //we spawn new mirrors.
        foreach (localKinectReceiver.KinectUtensilData kr in kinectResults)
        {
            if (kr.type == UtensilExtension.UtensilType.UNDETECTED) continue;
            GameObject spawnedIMirror = Instantiate(utensilMirrorPrefab);
            spawnedIMirror.transform.parent = StoneOrigin.transform;
            UtensilAbs mirrorUtensilBehaviour = spawnedIMirror.GetComponent<UtensilAbs>();
            mirrorUtensilBehaviour.RepositionRealGameobject(kr.position, kr.orientation);

            spawnedMirrors.Enqueue(spawnedIMirror);
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

   
}
