using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Kinect = Windows.Kinect;

/// <summary>
/// Handles rendering of the skeleton
/// </summary>
public class SkeletonVisualization : MonoBehaviour, IHideable
{
    public Material jointMaterial;
    public Material boneMaterialRed;
    public Material boneMaterialYellow;
    public Material boneMaterialGreen;

    public float jointDiameter = 0.06f;
    public float boneDiameter = 0.04f;

    ///used to draw lines between joints
    private Dictionary<Kinect.JointType, Kinect.JointType> m_jointParents = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineBase },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineBase },
        { Kinect.JointType.Neck, Kinect.JointType.SpineMid },
        { Kinect.JointType.Head, Kinect.JointType.Neck },

        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },

        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },

        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },

        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight }

        //{ Kinect.JointType.SpineShoulder, Kinect.JointType.SpineMid }
        
    };

    private Dictionary<Kinect.JointType, GameObject> m_jointGameObjects;
    private Dictionary<string, GameObject> m_boneGameObjects;
    private GameObject m_bodyGameObject;

    private bool m_isVisible = true;
    public bool Visible
    {
        get
        {
            return m_isVisible;
        }

        set
        {
            m_isVisible = value;

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach( Renderer renderer in renderers )
            {
                renderer.enabled = value;
            }
        }
    }

    private void Awake()
    {
        m_jointGameObjects = new Dictionary<Kinect.JointType, GameObject>();
        m_boneGameObjects = new Dictionary<string, GameObject>();

        ConstructBodyGameObject();
    }

    private void Start()
    {
        
    }


    ///update joints position based on received position (udp or any)
    public void SetJointPositions( Dictionary<Kinect.JointType, Vector3> jointPositions )
    {
        foreach (Kinect.JointType joint in jointPositions.Keys)
        {
            if (!m_jointGameObjects.ContainsKey(joint))
            {
                continue;
            }

            GameObject jointGameObject = m_jointGameObjects[joint];
            jointGameObject.transform.localPosition = jointPositions[joint];
        }

        //update transform of every joint and bones
        foreach (Kinect.JointType joint in jointPositions.Keys)
        {
            if (!m_jointParents.ContainsKey(joint))
                continue;

            Kinect.JointType parentJoint = m_jointParents[joint];
            if( parentJoint != joint )
            {
                string boneName = GetBoneName(parentJoint, joint);
                if (m_boneGameObjects.ContainsKey(boneName))
                {
                    GameObject boneObj = m_boneGameObjects[boneName];
                    Vector3 dir = GetJointGameObject(joint).transform.position - GetJointGameObject(parentJoint).transform.position;
                    float magnitude = dir.magnitude;

                    Vector3 boneScale = boneObj.transform.localScale;
                    boneScale.y = magnitude * 0.5f;
                    boneObj.transform.localScale = boneScale;
                    boneObj.transform.position = GetJointGameObject(parentJoint).transform.position + dir * 0.5f;
                    boneObj.transform.up = dir.normalized;
                }
            }
        }
    }

    public void SetBoneColorFromAngle( Kinect.JointType joint, float theta)
    {
        Kinect.JointType parentJoint = m_jointParents[joint];
        if (parentJoint != joint)
        {
            string boneName = GetBoneName(parentJoint, joint);

            if (m_boneGameObjects.ContainsKey(boneName) && theta <= 60)
            {
                GameObject boneObj = m_boneGameObjects[boneName];
                boneObj.GetComponent<Renderer>().sharedMaterial = boneMaterialRed;
            }
            else if (m_boneGameObjects.ContainsKey(boneName) && theta > 60 && theta <= 90)
            {
                GameObject boneObj = m_boneGameObjects[boneName];
                boneObj.GetComponent<Renderer>().sharedMaterial = boneMaterialYellow;
            }
            else if (m_boneGameObjects.ContainsKey(boneName) && theta > 90 && theta <= 180)
            {
                GameObject boneObj = m_boneGameObjects[boneName];
                boneObj.GetComponent<Renderer>().sharedMaterial = boneMaterialGreen;
            }
        }
    }

    public Vector3 GetJointWorldPosition(Kinect.JointType jointType)
    {
        Vector3 ret = Vector3.zero;
        if (m_jointGameObjects.ContainsKey(jointType))
        {
            ret = m_jointGameObjects[jointType].transform.position;
        }

        return ret;
    }

    public bool GetJointWorldPosition( Kinect.JointType jointType, out Vector3 jointWorldPos ) //same as above but gives also a check if the joint exists
    {
        if (m_jointGameObjects.ContainsKey(jointType))
        {
            jointWorldPos = m_jointGameObjects[jointType].transform.position;
            return true;
        }

        jointWorldPos = Vector3.zero;
        return false;
    }

    public GameObject GetJointGameObject( Kinect.JointType jointType )
    {
        if (m_jointGameObjects.ContainsKey(jointType))
        {
            return m_jointGameObjects[jointType];
        }

        return null;
    }

    //Creates the skeleton gameobject
    private void ConstructBodyGameObject() 
    {
        m_jointGameObjects.Clear();

        m_bodyGameObject = new GameObject("Body");
        m_bodyGameObject.transform.SetParent(this.transform, false);
        m_bodyGameObject.transform.localPosition = Vector3.zero;

        foreach ( Kinect.JointType childJoint in m_jointParents.Keys )
        {
            Kinect.JointType parentJoint = m_jointParents[childJoint];

            GameObject parentJointGameObject = null;

            //creates joints but with hierarchy
            if ( parentJoint != childJoint ) //if no parent will be set to itself
            {
                if (m_jointGameObjects.ContainsKey(parentJoint))
                {
                    parentJointGameObject = m_jointGameObjects[parentJoint];
                }
                else
                {
                    parentJointGameObject = CreateJointGameObject(parentJoint);
                    parentJointGameObject.transform.SetParent(m_bodyGameObject.transform, false);
                    parentJointGameObject.transform.localPosition = Vector3.zero;
                    
                    m_jointGameObjects.Add(parentJoint, parentJointGameObject);
                }
            }
            
            GameObject childJointGameObject = CreateJointGameObject( childJoint );
            childJointGameObject.transform.SetParent(m_bodyGameObject.transform, false);
            childJointGameObject.transform.localPosition = Vector3.zero;
            m_jointGameObjects.Add(childJoint, childJointGameObject);

            //creates bones between hierarchy joints
            if( parentJointGameObject != null )
            {
                GameObject boneObj = CreateBoneGameObject(parentJoint, childJoint);
                boneObj.transform.SetParent(m_bodyGameObject.transform, false);
                boneObj.transform.localPosition = Vector3.zero;
                m_boneGameObjects.Add(GetBoneName(parentJoint, childJoint), boneObj);
            }
        }
    }

    private GameObject CreateJointGameObject( Kinect.JointType jointType )
    {
        GameObject ret = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ret.transform.localScale = new Vector3(jointDiameter, jointDiameter, jointDiameter);
        ret.transform.localPosition = Vector3.zero;
        ret.name = jointType.ToString();
        ret.GetComponent<Renderer>().sharedMaterial = jointMaterial;

        // Just so that the joints don't add into physics checking
        Destroy(ret.GetComponent<Collider>());

        return ret;
    }

    private GameObject CreateBoneGameObject(Kinect.JointType jointA, Kinect.JointType jointB)
    {
        GameObject ret = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ret.transform.localScale = new Vector3(boneDiameter, 0.0f, boneDiameter);
        ret.transform.localPosition = Vector3.zero;
        ret.name = GetBoneName(jointA, jointB);
        ret.GetComponent<Renderer>().sharedMaterial = boneMaterialGreen;

        // Just so that the bones don't add into physics checking
        Destroy(ret.GetComponent<Collider>());

        return ret;
    }

    private string GetBoneName(Kinect.JointType jointA, Kinect.JointType jointB) //names the bone obj
    {
        if (jointA <= jointB)
        {
            return jointA.ToString() + "-" + jointB.ToString();
        }
        else
        {
            return jointB.ToString() + "-" + jointA.ToString();
        }
    }
}
