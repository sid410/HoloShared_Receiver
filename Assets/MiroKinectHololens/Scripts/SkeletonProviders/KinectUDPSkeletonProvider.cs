using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class KinectUDPSkeletonProvider : ISkeletonProvider
{
    private UDPHelper m_udpHelper;

    private Dictionary<Kinect.JointType, Vector3> m_jointPositions;

    protected override void Awake()
    {
        base.Awake();

        m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

        m_jointPositions = new Dictionary<Kinect.JointType, Vector3>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        m_udpHelper.MessageReceived += UDPMessageReceived;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        m_udpHelper.MessageReceived -= UDPMessageReceived;
    }

    private void UDPMessageReceived(NetInMessage message)
    {
        //Debug.Log("Skeleton received");
        int numJoints = message.ReadInt32();
        for( int i = 0; i < numJoints; i++)
        {
            Kinect.JointType jointType = (Kinect.JointType)message.ReadInt32();
            //Vector3 jointPosition = message.ReadVector3();
            Vector3 jointPosition;
            jointPosition.x = message.ReadFloat();
            jointPosition.y = message.ReadFloat();
            jointPosition.z = message.ReadFloat();

            m_jointPositions[jointType] = jointPosition;
        }
    }

    public override Dictionary<Kinect.JointType, Vector3> GetJointPositions()
    {
        return m_jointPositions;
    }
}
