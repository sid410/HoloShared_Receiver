using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity;
using SimpleJSON;
using Microsoft.MixedReality.Toolkit.UI;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using Vuforia;

public class MqttMessageHandler : MonoBehaviour
{
    public BaseClient baseClient;
    //public GameObject buttonMonitor, buttonCenter, buttonTag, sliderScale;
    //public GameObject buttonSetup, buttonPlan, buttonTool, buttonCarm, buttonReset, buttonNext, buttonPrevious, buttonShoot, buttonSendPos, buttonSendPic;
    public GameObject ImageTargetGO, PlatformTargetGO;
    public GameObject StonesOriginGO, PlatformOriginGO, PlatformGO;
    //private Interactable clickButtonMonitor, clickButtonCenter, clickButtonTag;
    //private PinchSlider valueSliderScale;
    //private Interactable clickButtonSetup, clickButtonPlan, clickButtonTool, clickButtonCarm, clickButtonReset, clickButtonNext, clickButtonPrevious, clickButtonShoot, clickButtonSendPos, clickButtonSendPic;
    //private MqttMsgPublish sendMsg = new MqttMsgPublish();

    private void Start()
    {
        PlatformGO.GetComponent<MeshRenderer>().enabled = true;

        //clickButtonMonitor = buttonMonitor.GetComponent<Interactable>();
        //clickButtonCenter = buttonCenter.GetComponent<Interactable>();
        //clickButtonTag = buttonTag.GetComponent<Interactable>();
        //valueSliderScale = sliderScale.GetComponent<PinchSlider>();

        //clickButtonSetup = buttonSetup.GetComponent<Interactable>();
        //clickButtonPlan = buttonPlan.GetComponent<Interactable>();
        //clickButtonTool = buttonTool.GetComponent<Interactable>();
        //clickButtonCarm = buttonCarm.GetComponent<Interactable>();
        //clickButtonReset = buttonReset.GetComponent<Interactable>();
        //clickButtonNext = buttonNext.GetComponent<Interactable>();
        //clickButtonPrevious = buttonPrevious.GetComponent<Interactable>();
        //clickButtonShoot = buttonShoot.GetComponent<Interactable>();
        //clickButtonSendPos = buttonSendPos.GetComponent<Interactable>();
        //clickButtonSendPic = buttonSendPic.GetComponent<Interactable>();

    }

    private void OnEnable()
    {
        baseClient.RegisterTopicHandler("M2MQTT/Surgery/CalibrateKinect", HandleCalibration);
        baseClient.RegisterTopicHandler("M2MQTT/Surgery/CalibratePlatform", HandleCalibration);
        //baseClient.RegisterTopicHandler("M2MQTT/Surgery/ImageView", HandleImageView);
        //baseClient.RegisterTopicHandler("M2MQTT/Surgery/MainMenu", HandleMainMenu);
    }

    private void OnDisable()
    {
        baseClient.UnregisterTopicHandler("M2MQTT/Surgery/CalibrateKinect", HandleCalibration);
        baseClient.UnregisterTopicHandler("M2MQTT/Surgery/CalibratePlatform", HandleCalibration);
        //baseClient.UnregisterTopicHandler("M2MQTT/Surgery/ImageView", HandleImageView);
        //baseClient.UnregisterTopicHandler("M2MQTT/Surgery/MainMenu", HandleMainMenu);
    }

    private void HandleCalibration(string topic, string message)
    {
        if (topic == "M2MQTT/Surgery/CalibrateKinect" && message == "true") StartCalibrationKinect();
        if (topic == "M2MQTT/Surgery/CalibrateKinect" && message == "false") StopCalibrationKinect();
        if (topic == "M2MQTT/Surgery/CalibratePlatform" && message == "true") StartCalibrationPlatform();
        if (topic == "M2MQTT/Surgery/CalibratePlatform" && message == "false") StopCalibrationPlatform();
    }

    //private void HandleImageView(string topic, string message)
    //{
    //    if (topic == "M2MQTT/Surgery/ImageView/Monitor" && message == "true") clickButtonMonitor.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/ImageView/Center" && message == "true") clickButtonCenter.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/ImageView/Tag" && message == "true") clickButtonTag.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/ImageView/Scale") valueSliderScale.SliderValue = float.Parse(message);
    //}

    //private void HandleMainMenu(string topic, string message)
    //{
    //    if (topic == "M2MQTT/Surgery/MainMenu/Setup" && message == "true") clickButtonSetup.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/MainMenu/Plan" && message == "true") clickButtonPlan.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/MainMenu/Tool" && message == "true") clickButtonTool.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/MainMenu/Carm" && message == "true") clickButtonCarm.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/MainMenu/Reset" && message == "true") clickButtonReset.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/MainMenu/Next" && message == "true") clickButtonNext.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/MainMenu/Previous" && message == "true") clickButtonPrevious.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/MainMenu/Shoot" && message == "true") clickButtonShoot.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/MainMenu/SendPos" && message == "true") clickButtonSendPos.TriggerOnClick();
    //    if (topic == "M2MQTT/Surgery/MainMenu/SendPic" && message == "true") clickButtonSendPic.TriggerOnClick();
    //}

    public void StartCalibrationKinect()
    {
        ImageTargetGO.SetActive(true);
        StartVuforiaCamera();
    }

    public void StopCalibrationKinect()
    {
        StonesOriginGO.transform.position = ImageTargetGO.transform.position;
        StonesOriginGO.transform.rotation = ImageTargetGO.transform.rotation;
        ImageTargetGO.SetActive(false);
        StopVuforiaCamera();
    }

    public void StartCalibrationPlatform()
    {
        PlatformTargetGO.SetActive(true);
        PlatformGO.transform.SetParent(PlatformTargetGO.transform);
        StartVuforiaCamera();
    }

    public void StopCalibrationPlatform()
    {
        PlatformOriginGO.transform.position = PlatformTargetGO.transform.position;
        PlatformOriginGO.transform.rotation = PlatformTargetGO.transform.rotation;
        PlatformGO.transform.SetParent(PlatformOriginGO.transform);
        PlatformTargetGO.SetActive(false);
        StopVuforiaCamera();
    }

    private void StartVuforiaCamera()
    {
        if (!Vuforia.CameraDevice.Instance.IsActive())
        {
            Vuforia.CameraDevice.Instance.Start();
        }
    }

    private void StopVuforiaCamera()
    {
        if (Vuforia.CameraDevice.Instance.IsActive())
        {
            Vuforia.CameraDevice.Instance.Stop();
        }
    }

    //private void PublishMessage(string topic, string message)
    //{
    //    sendMsg.Topic = topic;
    //    sendMsg.Message = Encoding.ASCII.GetBytes(message);
    //}
}