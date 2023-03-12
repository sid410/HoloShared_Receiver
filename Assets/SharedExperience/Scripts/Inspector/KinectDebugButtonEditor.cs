using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(localKinectReceiver))]
public class KinectDebugButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        localKinectReceiver ms = (localKinectReceiver)target;

        if (GUILayout.Button("Fake utensil Result"))
        {
            ms.HandleMatlabResults_debug();
        }
    }
}
