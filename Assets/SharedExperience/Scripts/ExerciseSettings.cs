using M2MqttUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciseSettings : MonoBehaviour
{
    public static ExerciseSettings Instance { private set; get; }

    [SerializeField] private BaseClient mqttClient;


    //Game options, modifiable but permanent
    [HideInInspector] public bool hintsEnabled { private set; get; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        hintsEnabled = true;
    }

    private void OnEnable()
    {
        mqttClient.RegisterTopicHandler("M2MQTT/enableHints", ChangeHintState);
    }

    private void OnDisable()
    {
        mqttClient.UnregisterTopicHandler("M2MQTT/enableHints", ChangeHintState);
    }

    //changes if hints are enabled or not in the exercise
    private void ChangeHintState(string topic, string message)
    {
        Debug.Log("Hints changing to " + message);
        try
        {
            hintsEnabled = bool.Parse(message);
        }
        catch (Exception e)
        {
            Debug.Log("error when parsing hints states" + e.Message);
        }

    }

}
