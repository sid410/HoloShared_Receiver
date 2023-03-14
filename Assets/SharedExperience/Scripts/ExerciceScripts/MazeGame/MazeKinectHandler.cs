using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UtensilExtension;


//handles kinect data for the maze exercise. Old items are destroyed and new prefabs are created
[CreateAssetMenu(fileName = "Assets/Resources/Data/KinectHandlers/MazeKinectH.asset", menuName = "ScriptableObjects/Kinect Handlers/Maze Kinect Handler", order = 2)]
public class MazeKinectHandler : KinectResultsAbs
{

    private class UtensilData
    {
        public UtensilAbs utensilScript;
        public int timeSinceLastDetection = detectionTimeout; //to prevent problem with bad detections, we only remove an item if it was undetected in a certain number of successive resutls.
    }
    private const int detectionTimeout = 3; 

    [Header("prefabs")]
    public GameObject utensilMirrorPrefab;
    public GameObject utensilRefactorPrefab;

    private GameObject StoneOrigin;
    Queue<GameObject> spawnedUtensilItems = new Queue<GameObject>();


    Dictionary<UtensilType, UtensilData> spawnedUtensilItemsV2 = new Dictionary<UtensilType, UtensilData>();

    private bool blockKinectResults = false;

    public override void Init()
    {
        StoneOrigin = GameObject.Find("StonesOrigin");
        spawnedUtensilItems = new Queue<GameObject>();
        spawnedUtensilItemsV2 = new Dictionary<UtensilType, UtensilData>();
        EventHandler.OnExerciseStepOver += OnExerciseStepOver;
        EventHandler.OnExerciseStepStarted += OnExerciseStepStarted; 
    }

    //setters for subscriptions
    private void OnExerciseStepStarted(ExerciceData.ExerciceStep es) => SetBlockKinectResults(false);

    private void OnExerciseStepOver() => SetBlockKinectResults(true); //we don't want the final kinect results to affect our items, we want the change of items to stop when the objectives are reached
    private void SetBlockKinectResults(bool newVal) => blockKinectResults = newVal;



    public  void HandleKinectDataV2(List<localKinectReceiver.KinectUtensilData> kinectResults)
    {
        if (blockKinectResults) return;

        DestroyOldMirrors(); //destroy old mirrors

        //we spawn new mirrors.
        foreach (localKinectReceiver.KinectUtensilData kr in kinectResults)
        {
            if (kr.type == UtensilType.UNDETECTED) continue;
            GameObject spawnedItem;
            float rotation;

            //we get the relevant data from the type of object
            switch (kr.type)
            {
                case UtensilType.CUP:
                case UtensilType.BOTTLE:
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
            spawnedUtensilItems.Enqueue(spawnedItem);
        }
    }

    public override void HandleKinectData(List<localKinectReceiver.KinectUtensilData> kinectResults)
    {
        if (blockKinectResults) return;

        int k = 0;
        List<UtensilType> alreadySpawnedUtensilsThisRound = new List<UtensilType>();

        //we spawn new mirrors.
        foreach (localKinectReceiver.KinectUtensilData kr in kinectResults)
        {
            UtensilType utensilType = kr.type;
            if (utensilType == UtensilType.DISH) utensilType = UtensilType.CUP;
            if (utensilType == UtensilType.UNDETECTED) continue;
            UtensilData spawnedItemData;
            float rotation;

            Debug.Log( k++ + " - Detected item of type " + utensilType);
            //we get the rotation based on the utensil type
            switch (utensilType)
            {
                case UtensilType.CUP:
                case UtensilType.DISH:
                case UtensilType.BOTTLE: 
                    rotation = 0;  
                    break;
                default: rotation = kr.orientation; break;
            }



            //we now try to find if the object was already instantiated before
            if (!spawnedUtensilItemsV2.TryGetValue(utensilType, out spawnedItemData))
            {
                GameObject itemSpawn;
                switch (utensilType)
                {
                    case UtensilType.CUP:
                    case UtensilType.BOTTLE:
                    case UtensilType.GLASS:
                    case UtensilType.DISH:
                        itemSpawn = Instantiate(utensilRefactorPrefab);
                        itemSpawn.transform.parent = StoneOrigin.transform;
                        break;
                    default:
                        itemSpawn = Instantiate(utensilMirrorPrefab);
                        itemSpawn.transform.parent = StoneOrigin.transform;
                        break;
                }

                spawnedItemData = new UtensilData();
                spawnedItemData.utensilScript = itemSpawn.GetComponent<MazeUtensil>();
                spawnedUtensilItemsV2.Add(utensilType, spawnedItemData);
            } else
            {

                /*Bandaid fix to problem of object recognition not correctly differntiating between cup and dish
                if (kr.type == UtensilType.CUP || kr.type == UtensilType.DISH)
                {
                    //we check the closest item
                    UtensilType otherType = kr.type == UtensilType.CUP ? UtensilType.DISH : UtensilType.CUP;
                    UtensilData otherUtensil;
                    bool otherTypeExists = spawnedUtensilItemsV2.TryGetValue(otherType, out otherUtensil); //we first get the other object

                    if (alreadySpawnedUtensilsThisRound.Contains(kr.type)) //we already adjusted the cup this round, let's check the dish
                    {
                        if (otherTypeExists) //the other similar utensil was already spawned ! lets adjust it.
                        {
                            spawnedItemData = otherUtensil;
                        } else
                        {
                            GameObject itemSpawn = Instantiate(utensilRefactorPrefab); // if we received double cup data, we spawn a dish instead.
                            itemSpawn.transform.parent = StoneOrigin.transform;
                            spawnedItemData = new UtensilData();
                            spawnedItemData.utensilScript = itemSpawn.GetComponent<MazeUtensil>();
                            spawnedUtensilItemsV2.Add(otherType, spawnedItemData);
                            alreadySpawnedUtensilsThisRound.Add(otherType);
                        }
                    } else //we never received this type this round ! we do nothign
                    {
                        alreadySpawnedUtensilsThisRound.Add(kr.type);
                    }
        }*/


                if (!spawnedItemData.utensilScript.gameObject.activeSelf) spawnedItemData.utensilScript.gameObject.SetActive(true); //we reenable the item if it is disabled
            }

            //we reposition the item
            UtensilAbs utensilBehaviour = spawnedItemData.utensilScript;
            utensilBehaviour.RepositionRealGameobject(kr.position, rotation);
            spawnedItemData.timeSinceLastDetection = detectionTimeout; //we reset the time since we last saw these items


        }

        //we now reduce all items by 1 => we disable those that have disappeared for too long
        foreach (UtensilData udata in spawnedUtensilItemsV2.Values)
        {
            if (--udata.timeSinceLastDetection <= 0)
            {
                udata.utensilScript.gameObject.SetActive(false);
            }
        }
    }


    private void DestroyOldMirrors()
    {
        if (spawnedUtensilItems == null || spawnedUtensilItems.Count == 0) return;

        while (spawnedUtensilItems.Count > 0)
        {
            GameObject mirrorObj = spawnedUtensilItems.Dequeue();
            Destroy(mirrorObj.gameObject);
        }
    }

    public override void Cleanup()
    {
        EventHandler.OnExerciseStepOver -= OnExerciseStepOver;
        EventHandler.OnExerciseStepStarted -= OnExerciseStepStarted;
    }
}
