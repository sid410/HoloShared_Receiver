using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity;
using System;
using Vuforia;

/**
 * Testing class for instantiating object with a single hololens.
 * THIS CLASS SHOULDNT BE ENABLED OR USED FOR RELEASE VERSIONS
 */
public class LocalItemSpawner : MonoBehaviour 
{
    //Boolean used to disable enable behaviours related to single hololens
    public static bool SINGLE_HOLO_BEHAVIOUR = true;

    [Header("UI swapping vars")]
    public GameObject[] UIPrefabs; //Contains multiple types of prefab for testing different UI arrangements
    public int EnabledUIIndex = 0;


    public GameObject[] Utensils;

    [Tooltip("Use this to fake a live detected object. This is replaced with live tracking of real object")]
    public GameObject debugSenderPrefab;
    public GameObject StoneOrigin;
    public FullExerciceHandler exerciceHandler; //Handles the steps of an exercise, we use it to trigger the startof an exercise

    private GameObject currentEnabledUI;
    

    private OSCTransmitter Transmitter;
    [Header("Utensil spawn entry, contains positions and utensils to spawn. This is for debugging and spawning a defautl spawn entry")]
    public ItemsSpawnEntry debug_spawnEntry;

    private bool exercice_initialised = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!SINGLE_HOLO_BEHAVIOUR) //if we are not running in debug mode, we destroy this object
        {
            Destroy(gameObject);
            return;
        }
        
        Transmitter = GameObject.FindObjectOfType<OSCTransmitter>();
        SetRemoteHost("127.0.0.1"); //try to send receiver to localHost
        SpawnUI(); //we spawn the UI, 


       //StartCoroutine(TriggerExerciceStart());

        EventHandler.calibrationDone += (p,r) => { StartCoroutine(TriggerExerciceStart()); }; //for debugging purposes we trigger the exercice start after 4 seconds

    }

    //Debug Coroutine to trigger an exercice starting 
    IEnumerator TriggerExerciceStart()
    {
        if (!exercice_initialised)
        {
            exercice_initialised = true;
            //yield return new WaitForSeconds(2f);
            //SendSpawnUstensilsRequests(); //we spawn the utensils
            //yield return new WaitForSeconds(2f);
            //SendUpdatedTransform();
            yield return new WaitForSeconds(2f);
            exerciceHandler.DebugStartExercise(); 
        }
    }

    
    //sets the address.
    private void SetRemoteHost(string rcvrAddress)
    {
        Transmitter.RemoteHost = rcvrAddress;
        Transmitter.RemotePort = 7000;
    }

    /**
     * Spawns usensils without the Use of another hololive. This is a function purely for 1 hololens testing.
     * This basically sends a request to itself.
     */
    public void SendSpawnUstensilsRequests()
    {
        if (debug_spawnEntry == null) return;
        var objListMsg = new OSCMessage("/spawnObject");
        EventHandler.Instance.OnLog("Spawning objects");
        objListMsg.AddValue(OSCValue.Int(debug_spawnEntry.spawnPoints.Count)); //objectSpawner.GetCollidingGameObjectsList()

        int i = 0;
        foreach (ItemsSpawnEntry.SpawnPoint utensilSpawnPoint in debug_spawnEntry.spawnPoints) //objectSpawner.GetCollidingGameObjectsList()
        {
            objListMsg.AddValue(OSCValue.String(utensilSpawnPoint.itemPrefab.tag));
            objListMsg.AddValue(OSCValue.String(utensilSpawnPoint.itemPrefab.name + "_" + i++));
        }

        Transmitter.Send(objListMsg);
        //StartCoroutine(InitializeTransformDelayed(0.2f));
    }


    public void TriggerExerciseStart(FullExerciceData exercise)
    {
        //Receiver.Bind("/loadexercise", LoadExercice); //we bind to the receiver, this will trigger the exercise from the phone app eventually.
        var exerciseTriggerMessage = new OSCMessage("/loadexercise");
        exerciseTriggerMessage.AddValue(OSCValue.Int((int)exercise.exerciseType)); //we simply pass the exercise's enum number, should be consistent betweens apps
        EventHandler.Instance.OnLog("Triggering the start of the exercise " + exercise.exerciseType);
        Transmitter.Send(exerciseTriggerMessage);
    }

    //Sends the updated transform locatinos for the utensils (TODO : uncomplete, must be put in gameobjects, but probably unecessary we don't need this for testing)
    public void SendUpdatedTransform()
    {
        string parentName = "StonesOrigin/";
        

        int i = 0;
        foreach (ItemsSpawnEntry.SpawnPoint utensilSpawnPoint in debug_spawnEntry.spawnPoints) //objectSpawner.GetCollidingGameObjectsList()
        { //TODO : rework this to be usable for any type of prefab, no juste utensils (maybe with spawnable prefabs dictionnary)
            var trasformStructure = new TransformMarshallingStructure();
            string adress = parentName + utensilSpawnPoint.itemPrefab.name + "_" + i++;
            var message = new OSCMessage(adress);
            trasformStructure.PosValue = new Vector3(utensilSpawnPoint.PosX, 0, -utensilSpawnPoint.PosY);
            trasformStructure.RotValue = Quaternion.Euler(utensilSpawnPoint.rotation);
            EventHandler.Instance.OnLog("Sending position " + trasformStructure.PosValue.ToString() + " to adress " + adress);
            var bytes = OSCUtilities.StructToByte(trasformStructure);
            message.AddValue(OSCValue.Blob(bytes));

            Transmitter.Send(message);
        }
        
    }

    public void SpawnDebugSenderItem()
    {
        if (debugSenderPrefab == null) return;
        EventHandler.Instance.OnLog("Spawning holdable cup");
        GameObject SenderPrefab = Instantiate(debugSenderPrefab, StoneOrigin.transform.position, Quaternion.identity);
        SenderPrefab.transform.parent = StoneOrigin.transform;
    }

    //starts the calibration of the table
    public void SendStartCalibration()
    {
        var objListMsg = new OSCMessage("/CalibrateSharedSpace/Receiver");
        EventHandler.Instance.OnLog("Starting calibration");
        objListMsg.AddValue(OSCValue.Bool(true)); //objectSpawner.GetCollidingGameObjectsList()
        Transmitter.Send(objListMsg);
        //StartCoroutine(SpamFunctionAFterDelay());
    }

    //end the calibration of the table
    public void SendEndCalibration()
    {
        var objListMsg = new OSCMessage("/CalibrateSharedSpace/Receiver");
        EventHandler.Instance.OnLog("Ending Calibration");
        objListMsg.AddValue(OSCValue.Bool(false)); //objectSpawner.GetCollidingGameObjectsList()
        Transmitter.Send(objListMsg);
        //StopAllCoroutines();
    }


    //************************ UI Swap scripts *******************************//

    //we swap between UIs
    public void GoToNextUI()
    {
        if (UIPrefabs == null || UIPrefabs.Length == 0) return;

        EnabledUIIndex = (EnabledUIIndex + 1) % UIPrefabs.Length;
        SpawnUI();
    }


    private void SpawnUI()
    {
        if (UIPrefabs == null || UIPrefabs.Length == 0) return;
        if (currentEnabledUI != null) Destroy(currentEnabledUI);

        currentEnabledUI = Instantiate(UIPrefabs[EnabledUIIndex]);
        currentEnabledUI.transform.parent = StoneOrigin.transform; //we set the parent as the stone origin
        currentEnabledUI.transform.localPosition = Vector3.zero; //we set the location as 0

    }
}