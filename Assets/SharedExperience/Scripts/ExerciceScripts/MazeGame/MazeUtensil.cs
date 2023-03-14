using M2MqttUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeUtensil : UtensilAbs
{
    private BaseClient client;

    [SerializeField] private MeshRenderer mesh;
    [Header("This is a correction used when the angle is very close to 45 / 90 / 0 degrees, This is to decrease insatisfaction")]
    [SerializeField] private float perfectAngleMargin = 2.5f;
    private float[] perfectAngles = { 0, 45, 90, 135, 180 };
    private void Start()
    {
        EventHandler.Instance.SpawnItem(this.gameObject);
    }

    private void OnEnable()
    {
        client = GameObject.Find("BaseClient").GetComponent<BaseClient>();
        //client.RegisterTopicHandler("M2MQTT/debugbtn", Debug_ShowModel);
    }

    private void OnDisable()
    {
        //client.UnregisterTopicHandler("M2MQTT/debugbtn", Debug_ShowModel);
    }

    private void Debug_ShowModel(string topic, string message)
    {
        Debug.Log("received a message for displaying the mesh model " + message);
        bool DisplayModel = bool.Parse(message);
        if (mesh != null) mesh.enabled = DisplayModel;
    }
    //we reposition according to the table
    public override void RepositionRealGameobject(Vector3 tablePos, float tableRot)
    {
        EventHandler.Instance.LogMessage("Repositioning real gameobject ! " + gameObject.name);
        gameObject.transform.localPosition = tablePos;
        if (type == UtensilExtension.UtensilType.CUP && tablePos.x > 0.45f)
        {
            gameObject.transform.localPosition += new Vector3((tablePos.x - 0.45f) * 0.15f, 0, 0);
        }

        //we correct angle if it's close from a basic good angle
        if (tableRot != 0) tableRot = FixAngle(tableRot);

        //we set the rotation finally
        if (tableRot != 0) gameObject.transform.localEulerAngles = new Vector3(0, tableRot, 0);
    }


    //takes a received angle from kinect, sees if its really close from the passed perfect angle and fixes it if it's close
    private float FixAngle(float tableRot)
    {
        //we want the angular value to be between 0 and 180 since the face is symmetrical (one face or another doesn't matter)
        float adjustedAngle = tableRot % 180;
        if (adjustedAngle < 0) adjustedAngle += 180;

        for (int i = 0; i < perfectAngles.Length; i++)
        {
            if (Mathf.Abs(perfectAngles[i] - adjustedAngle) < perfectAngleMargin)
            {
                adjustedAngle = perfectAngles[i];
                return adjustedAngle;
            }
        }

        return adjustedAngle;
    }
}
