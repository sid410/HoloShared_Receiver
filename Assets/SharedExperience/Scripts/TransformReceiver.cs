using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC.Examples;
using extOSC;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct TransformMarshallingStructure
{
    public Vector3 PosValue;
    public Quaternion RotValue;
}

public class TransformReceiver : MonoBehaviour
{
    private OSCReceiver Receiver;
    private string _address;

    private void Start()
    {
        _address = GetFullName(this.gameObject);
        Receiver = GameObject.FindObjectOfType<OSCReceiver>();
        Receiver.Bind(_address, UpdateTransform);
    }

    private void UpdateTransform(OSCMessage message)
    {
        byte[] bytes;
        
        if (!message.ToBlob(out bytes))
            return;
        
        var trasformStructure = OSCUtilities.ByteToStruct<TransformMarshallingStructure>(bytes);

        transform.localPosition = trasformStructure.PosValue;
        transform.localRotation = trasformStructure.RotValue;
        
    }

    private static string GetFullName(GameObject go)
    {
        string name = go.name;
        while (go.transform.parent != null)
        {

            go = go.transform.parent.gameObject;
            name = go.name + "/" + name;
        }
        return name;
    }
}
