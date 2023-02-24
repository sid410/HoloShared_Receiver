using M2MqttUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;
using static AppStateHandler;


//This script's role is broadcasting the states of the app to the network. Used for the node-red webapp to display data.
public class DataBroadCaster : MonoBehaviour
{
    [SerializeField] private BaseClient client;

    //consts
    //topics
    private const string appStateTopic = "M2MQTT/state";

    private void OnEnable()
    {
        EventHandler.OnTutorialStarted += InformTutorialStarted;
        EventHandler.OnExerciseStarted += InformExerciseStarted;
        EventHandler.OnExerciseOver += InformResultsStarted;
    }

    private void OnDisable()
    {
        EventHandler.OnTutorialStarted -= InformTutorialStarted;
        EventHandler.OnExerciseStarted -= InformExerciseStarted;
        EventHandler.OnExerciseOver -= InformResultsStarted;
    }

    //we inform any listeners that the current status is the tutorial
    private void InformTutorialStarted(TutorialData td) => InformOfAppState("Tutorial in progress");

    private void InformExerciseStarted(ExerciceData ed) => InformOfAppState("Task in progress");

    private void InformResultsStarted() => InformOfAppState("Displaying results");

    private void InformOfAppState(string state) => client.PublishMessage(appStateTopic, System.Text.Encoding.UTF8.GetBytes(state), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
}
