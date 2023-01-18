using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity;
using SimpleJSON;
using Microsoft.MixedReality.Toolkit.UI;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using Vuforia;
using extOSC;

public class SharedXPControlInterface : MonoBehaviour
{
    private AnchorableObject originAnchor;
    private OSCReceiver Receiver;

    public BaseClient baseClient;
    public GameObject ImageTargetGO;
    public GameObject StonesOriginGO;
    public GameObject UtensilParent;
    public GameObject TableOriginGO;
    public GameObject[] Utensils;

    private void Start()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(StopVuforiaCamera);
        originAnchor = TableOriginGO.GetComponent<AnchorableObject>();

        Receiver = GameObject.FindObjectOfType<OSCReceiver>();
        //Receiver.Bind("/destroyObjects", DestroyAllExistingUtensils);
        Receiver.Bind("/spawnObject", SpawnUtensilFromOtherHolo);

        //DEBUG : disable later
        Receiver.Bind("/CalibrateSharedSpace/Receiver", (message) =>
        {
            Debug.Log("received calibration DEBUG message");
            bool value = message.Values[0].BoolValue;
            if (value) StartCalibration();
            else StopCalibration();
        });
    }

    private void OnEnable()
    {
        baseClient.RegisterTopicHandler("M2MQTT/CalibrateSharedSpace/Receiver", HandleCalibration);
        baseClient.RegisterTopicHandler("M2MQTT/DestroyObjectsList", DestroyAllExistingUtensils);
    }

    private void OnDisable()
    {
        baseClient.UnregisterTopicHandler("M2MQTT/CalibrateSharedSpace/Receiver", HandleCalibration);
        baseClient.UnregisterTopicHandler("M2MQTT/DestroyObjectsList", DestroyAllExistingUtensils);
    }

    private void HandleCalibration(string topic, string message)
    {
        if (topic == "M2MQTT/CalibrateSharedSpace/Receiver" && message == "true") StartCalibration();
        if (topic == "M2MQTT/CalibrateSharedSpace/Receiver" && message == "false") StopCalibration();
    }

    public void StartCalibration()
    {
        Debug.Log("Received a start calibration message");
        ImageTargetGO.SetActive(true);
        StartVuforiaCamera();

    }

    public void StopCalibration()
    {
        if (LocalItemSpawner.SINGLE_HOLO_BEHAVIOUR)
        {
            EventHandler.Instance.LogMessage("Calibration yielded new transform : " + ImageTargetGO.transform.position.ToString());
            EventHandler.Instance.LogMessage("Calibration yielded new rotation : " + ImageTargetGO.transform.rotation.ToString());
        }
        TableOriginGO.transform.position = ImageTargetGO.transform.position;
        TableOriginGO.transform.rotation = ImageTargetGO.transform.rotation;

        originAnchor.SaveAnchor();

        ImageTargetGO.SetActive(false);

        EventHandler.Instance.TriggerCalibrationEnded(ImageTargetGO.transform.position, ImageTargetGO.transform.rotation); //we inform the rest of the app that the calibration is over.
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

    private void DestroyAllExistingUtensils(string topic, string message)
    {
        if (topic == "M2MQTT/DestroyObjectsList")
        {
            List<GameObject> children = GetAllChilds(StonesOriginGO);
            foreach (GameObject child in children)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    private void SpawnUtensilFromOtherHolo(OSCMessage message)
    {
        var numUtensils = message.Values[0].IntValue;

        for (int i=1; i<=numUtensils*2; i+=2)
        {
            int tagInt = ConvertUtensilNameToInt(message.Values[i].StringValue);
            if (tagInt == -1) return;

            GameObject myUtensil = Instantiate(Utensils[tagInt]) as GameObject;
            myUtensil.name = message.Values[i + 1].StringValue;
            myUtensil.transform.parent = UtensilParent.transform;
    
            if (LocalItemSpawner.SINGLE_HOLO_BEHAVIOUR) myUtensil.transform.localPosition = new Vector3(0.2f + ((tagInt == 4)? 0.4f :tagInt * 0.2f), 0, -0.3f); //TODO : here test for spawning accuracy
        }
    }

    private int ConvertUtensilNameToInt(string name)
    {
        switch (name)
        {
            case "Spoon":
                return 0;
            case "Fork":
                return 1;
            case "Cup":
                return 2;
            case "Dish":
                return 3;
            case "Knife":
                return 4;
            case "Minispoon":
                return 5;
            case "Bottle":
                return 6;
            case "Glass":
                return 7;
            default:
                return -1;

        }
    }

    private static List<GameObject> GetAllChilds(GameObject Go)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < Go.transform.childCount; i++)
        {
            list.Add(Go.transform.GetChild(i).gameObject);
        }
        return list;
    }
}
