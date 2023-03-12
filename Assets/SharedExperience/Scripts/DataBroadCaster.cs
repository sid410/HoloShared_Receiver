using M2MqttUnity;
using System;
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
    private const string logsTopic = "M2MQTT/logs";

    private void OnEnable()
    {
        EventHandler.OnTutorialStarted += InformTutorialStarted;
        EventHandler.OnExerciseStarted += InformExerciseStarted;
        EventHandler.OnExerciseOver += InformResultsStarted;
        EventHandler.OnLog += BroadCastLogs;
    }

    private void OnDisable()
    {
        EventHandler.OnTutorialStarted -= InformTutorialStarted;
        EventHandler.OnExerciseStarted -= InformExerciseStarted;
        EventHandler.OnExerciseOver -= InformResultsStarted;
        EventHandler.OnLog -= BroadCastLogs;
    }

    //we inform any listeners that the current status is the tutorial
    private void InformTutorialStarted(TutorialData td) => InformOfAppState("Tutorial in progress");

    private void InformExerciseStarted(ExerciceData ed) => InformOfAppState("Task in progress");

    private void InformResultsStarted() => InformOfAppState("Displaying results");

    private void InformOfAppState(string state) => client.PublishMessage(appStateTopic, System.Text.Encoding.UTF8.GetBytes(state), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);


    private void BroadCastLogs(string log)
    {
        Debug.Log("publishing log " + log);
        client.PublishMessage(logsTopic, System.Text.Encoding.UTF8.GetBytes(DateTime.Now.TimeOfDay.ToString() + ": " + log), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
    }
}
