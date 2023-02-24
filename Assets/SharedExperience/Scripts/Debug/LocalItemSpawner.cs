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

    [Header("Client M2QTT")]
    public BaseClient client;

    [Header("UI swapping vars")]
    public GameObject debug_buttons;
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
        client.RegisterTopicHandler("M2MQTT/debugbtn", ChangeDebugButtonStatus); //we bind to the receiver, this will trigger the exercise from the phone app eventually
        SpawnUI();
    }


    public void StartUtensilExercise() => TriggerExerciseStart(ExerciseType.UTENSIL);

    public void StartMazeExercise() => TriggerExerciseStart(ExerciseType.MAZE);

    //sets the address.
    private void SetRemoteHost(string rcvrAddress)
    {
        Transmitter.RemoteHost = rcvrAddress;
        Transmitter.RemotePort = 7000;
    }


    //Received from the node-red web app, disables/enables the UI buttons
    private void ChangeDebugButtonStatus(string topic, string message)
    {
        Debug.Log("received a message for debug buttons " + message);
        bool displayDebugButtons = bool.Parse(message);
        debug_buttons.SetActive(displayDebugButtons);
    }
    /**
     * Spawns usensils without the Use of another hololive. This is a function purely for 1 hololens testing.
     * This basically sends a request to itself.
     */
    public void SendSpawnUstensilsRequests()
    {
        if (debug_spawnEntry == null) return;
        var objListMsg = new OSCMessage("/spawnObject");
        EventHandler.Instance.LogMessage("Spawning objects");
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

    //hardcoded way of triggering the mazes being over
    public void TriggerExerciseOver()
    {
        StartCoroutine(TriggerMazeSolutionCoroutine(1, 1f));
        //StartCoroutine(TriggerMazeSolutionCoroutine(2, 10f));
    }

    //debug function : triggers multiple events to try to reach the end of an exercise
    IEnumerator TriggerMazeSolutionCoroutine(int goals, float initialWaitTime)
    {
        yield return new WaitForSeconds(initialWaitTime);
        EventHandler.Instance.TriggerBeforeMatlabReceived();
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < goals; i++)
        {
            EventHandler.Instance.SetObjectiveAsComplete(MazeObjectives.INIT_OBJECTIVE_INDEX, this.gameObject);
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
        EventHandler.Instance.TriggerFinalMatlabReceived();
        EventHandler.Instance.TriggerFinalMatlabProcessed(); //used to announce data was processed (for results)


    }


    public void TriggerExerciseStart(ExerciseType exerciseType)
    {
        ResetApp();
        //Receiver.Bind("/loadexercise", LoadExercice); //we bind to the receiver, this will trigger the exercise from the phone app eventually.
        var exerciseTriggerMessage = new OSCMessage("/loadexercise");
        exerciseTriggerMessage.AddValue(OSCValue.Int((int)exerciseType)); //we simply pass the exercise's enum number, should be consistent betweens apps
        EventHandler.Instance.LogMessage("Triggering the start of the exercise " + exerciseType);
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
            EventHandler.Instance.LogMessage("Sending position " + trasformStructure.PosValue.ToString() + " to adress " + adress);
            var bytes = OSCUtilities.StructToByte(trasformStructure);
            message.AddValue(OSCValue.Blob(bytes));

            Transmitter.Send(message);
        }
        
    }

    public void SpawnDebugSenderItem()
    {
        if (debugSenderPrefab == null) return;
        EventHandler.Instance.LogMessage("Spawning holdable cup");
        GameObject SenderPrefab = Instantiate(debugSenderPrefab, StoneOrigin.transform.position, Quaternion.identity);
        SenderPrefab.transform.parent = StoneOrigin.transform;
    }

    //starts the calibration of the table
    public void SendStartCalibration()
    {
        var objListMsg = new OSCMessage("/CalibrateSharedSpace/Receiver");
        EventHandler.Instance.LogMessage("Starting calibration");
        objListMsg.AddValue(OSCValue.Bool(true)); //objectSpawner.GetCollidingGameObjectsList()
        Transmitter.Send(objListMsg);
        //StartCoroutine(SpamFunctionAFterDelay());
    }

    //end the calibration of the table
    public void SendEndCalibration()
    {
        var objListMsg = new OSCMessage("/CalibrateSharedSpace/Receiver");
        EventHandler.Instance.LogMessage("Ending Calibration");
        objListMsg.AddValue(OSCValue.Bool(false)); //objectSpawner.GetCollidingGameObjectsList()
        Transmitter.Send(objListMsg);
        //StopAllCoroutines();
    }

    //Resets the app to the state it is at the start (no exercise loaded)
    public void ResetApp()
    {
        EventHandler.Instance.ResetApp();
    }

    //************************ UI Swap scripts *******************************//

    //we swap between UIs
    public void GoToNextUI()
    {
        if (UIPrefabs == null || UIPrefabs.Length == 0) return;

        EnabledUIIndex = (EnabledUIIndex + 1) % UIPrefabs.Length;
        SpawnUI();
    }


    public void GoToPreviousUI()
    {
        if (UIPrefabs == null || UIPrefabs.Length == 0) return;

        EnabledUIIndex = ((EnabledUIIndex - 1) < 0) ? UIPrefabs.Length - 1 : EnabledUIIndex - 1;
        SpawnUI();

    }

    //Changed this to not spawn but disable enable UIs so subscriptions don't disppear and dada is passed
    private void SpawnUI()
    {
        if (UIPrefabs == null || UIPrefabs.Length == 0) return;
        if (currentEnabledUI != null) Destroy(currentEnabledUI.gameObject);

        currentEnabledUI = Instantiate(UIPrefabs[EnabledUIIndex]);
        currentEnabledUI.transform.parent = StoneOrigin.transform; //we set the parent as the stone origin
        currentEnabledUI.transform.localPosition = new Vector3(0, 0, 0.02f); //we set the location as 0
        currentEnabledUI.transform.localRotation = Quaternion.identity;

    }
}
