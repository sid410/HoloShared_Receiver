using UnityEngine;

using M2MqttUnity;

using SimpleJSON;

using System.Collections.Generic;

public class SkeletonManager : MonoBehaviour
{
    public BaseClient baseClient;
    public ISkeletonProvider skeletonProvider;
    public SkeletonVisualization skeletonVisualization;

    public bool ElbowLeftFlag;
    public bool ElbowRightFlag;
    public bool KneeLeftFlag;
    public bool KneeRightFlag;
    public bool FeetTrajectoriesFlag;
    public bool HeadTrajectoryFlag;
    public bool VelocityFlag;
    public bool RoadFlag;
    public bool HeightFlag;

    public int HeartRate;
    public double BodyTemp;
    public double EMGData;

    void Update()
    {
        UpdateAvatarVisualization();
    }

    private void OnEnable()
    {
        baseClient.RegisterTopicHandler("M2MQTT/WebApp", HandleWebAppMqttMessage);
        baseClient.RegisterTopicHandler("M2MQTT/HeartbeatData", HandleHeartbeatMessage);
        baseClient.RegisterTopicHandler("M2MQTT/BodyTemperatureData", HandleBodyTemperatureMessage);
        baseClient.RegisterTopicHandler("M2MQTT/EMGData", HandleEMGDataMessage);
    }

    private void OnDisable()
    {
        baseClient.UnregisterTopicHandler("M2MQTT/WebApp", HandleWebAppMqttMessage);
        baseClient.UnregisterTopicHandler("M2MQTT/HeartbeatData", HandleHeartbeatMessage);
        baseClient.UnregisterTopicHandler("M2MQTT/BodyTemperatureData", HandleBodyTemperatureMessage);
        baseClient.UnregisterTopicHandler("M2MQTT/EMGData", HandleEMGDataMessage);
    }

    private void UpdateSkeletonVisualization()
    {
        if (skeletonVisualization == null || skeletonProvider == null)
        {
            return;
        }

        Dictionary<Windows.Kinect.JointType, Vector3> jointPositions = skeletonProvider.GetJointPositions();
        skeletonVisualization.SetJointPositions(jointPositions);

    }

    private void UpdateAvatarVisualization()
    {
        if (skeletonVisualization == null || skeletonProvider == null)
        {
            return;
        }

        Dictionary<Windows.Kinect.JointType, Vector3> jointPositions = skeletonProvider.GetJointPositions();
        skeletonVisualization.SetJointPositions(jointPositions);

    }

    private void HandleWebAppMqttMessage(string topic, string message)
    {
        Debug.Log(message);
        JSONNode webAppNode = JSON.Parse(message);
        if (webAppNode == null)
        {
            return;
        }

        if (webAppNode["flag"] != null)
        {
            JSONNode flagNode = webAppNode["flag"];
            string flagName = flagNode["name"].Value;
            bool flag = flagNode["value"].AsBool;

            if (flagName.Equals("Skeleton"))
            {
                skeletonVisualization.Visible = flag;
            }
            else if (flagName.Equals("JointElbowLeft"))
            {
                ElbowLeftFlag = flag;
            }
            else if (flagName.Equals("JointElbowRight"))
            {
                ElbowRightFlag = flag;
            }
            else if (flagName.Equals("JointKneeLeft"))
            {
                KneeLeftFlag = flag;
            }
            else if (flagName.Equals("JointKneeRight"))
            {
                KneeRightFlag = flag;
            }
            else if (flagName.Equals("FeetTrajectories"))
            {
                FeetTrajectoriesFlag = flag;
            }
            else if (flagName.Equals("HeadTrajectory"))
            {
                HeadTrajectoryFlag = flag;
            }
            else if (flagName.Equals("Velocity"))
            {
                VelocityFlag = flag;
            }
            else if (flagName.Equals("Road"))
            {
                RoadFlag = flag;
            }
            else if (flagName.Equals("Height"))
            {
                HeightFlag = flag;
            }
        }
    }

    private void HandleHeartbeatMessage(string topic, string message)
    {
        Debug.Log("heartrate = " + message);
        HeartRate = System.Convert.ToInt32(message);
    }

    private void HandleBodyTemperatureMessage(string topic, string message)
    {
        Debug.Log(message);
        BodyTemp = System.Convert.ToDouble(message);
    }

    private void HandleEMGDataMessage(string topic, string message)
    {
        Debug.Log(message);
        EMGData = System.Convert.ToDouble(message);
    }

}
