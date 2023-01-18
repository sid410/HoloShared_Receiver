using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtensilBehaviour : UtensilAbs
{
    public static int st_itemID = 0; //static iD, increased for each intansiated object of this type

    public Material objectiveComplete_Material;
    public GameObject realGoPrefab; //
    public bool angledUtensil = true;


    private Material defaultMaterial; //default utensil material
    private MeshRenderer meshRenderer;
    private GameObject realGoInstance;

    private GameObject virtualForward, realForward, virtualOrigin, realOrigin;

    //TODO : Think about how to reset data when a new matlab event comes.
    public class ResultPack
    {
        public string utensilName = "default";
        public float _angleError = -1;
        public float _distanceError = -1;
        public bool succeeded = false;
    }

    public ResultPack resultPack { private set; get; } //pack stores the data

    private bool realItemDetected = false; //we detect if the real item was ever detected, this enables collision behaviours
    public int itemID { get; private set; }
    private void Start()
    {
        resultPack = new ResultPack(); //we instantiate a result pack which encapsulates the results
        resultPack.utensilName = gameObject.name;
        meshRenderer = GetComponent<MeshRenderer>();
        defaultMaterial = meshRenderer.material; // we get the default material for this from the meshrenderer
        itemID = st_itemID++; //we set our ID and increment this.

        InstantiateRealObject();
        EventHandler.Instance.SpawnItem(this.gameObject);
    }
    
    //instantiates the real gameobject
    private void InstantiateRealObject()
    {
        GameObject StonesOrigin = GameObject.Find("StonesOrigin");

        if (realGoPrefab == null) return;
        realGoInstance = Instantiate(realGoPrefab);
        realGoInstance.transform.parent = StonesOrigin.transform;
        realGoInstance.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + 0.1f, 0, gameObject.transform.localPosition.z);

        virtualOrigin = gameObject.transform.Find("origin").gameObject;
        realOrigin = realGoInstance.transform.Find("origin").gameObject;

        if (!angledUtensil) return;

        //for angled object we get the forward direction
        virtualForward = gameObject.transform.Find("Xaxis").transform.Find("forwardDir").gameObject;
        realForward = realGoInstance.transform.Find("Xaxis").transform.Find("forwardDir").gameObject;
        EventHandler.Instance.LogMessage("Finished Instantiating real object ! " + gameObject.name);
    }

    //uses data received from the kinect to reposition the Gameobject tied to this
    public override void RepositionRealGameobject(Vector3 tablePos, float tableRot)
    {
        EventHandler.Instance.LogMessage("Repositioning real gameobject ! " + gameObject.name);
        if (realGoInstance == null) return;
        realItemDetected = true;
        realGoInstance.transform.localPosition = tablePos;
        if (angledUtensil) realGoInstance.transform.localEulerAngles = new Vector3(0, tableRot, 0);

        CalculateAngleError();
        CalculatePositionError();
    }

    //calculates the positino error in relation to the gameobject
    private void CalculatePositionError()
    {
        float dist = Vector3.Distance(virtualOrigin.transform.position, realOrigin.transform.position) * 1000;
        ///distanceText.text = dist.ToString("N1") + "mm";
        resultPack._distanceError = dist;
    }

    //calculates the angle error in relation to the objective
    private void CalculateAngleError()
    {
        if (!angledUtensil) return;
        Vector3 virtualDir = virtualForward.transform.position - virtualOrigin.transform.position;
        Vector3 realDir = realForward.transform.position - realOrigin.transform.position;
        float angle = Vector3.Angle(virtualDir, realDir);
        if (angle > 90.0f) angle = 180.0f - angle;

        resultPack._angleError = angle;
    }


    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Trigger entered");
        if (!realItemDetected) return;
        LiveModelCollision hitModel = collision.gameObject.GetComponent<LiveModelCollision>();
        if (hitModel == null) return;
        if (hitModel.liveModelType == this.type)
        {
            resultPack.succeeded = true;
            meshRenderer.material = objectiveComplete_Material;
            //old
            //EventHandler.Instance.OnUtensilObjectiveCompleted(this);

            //new
            EventHandler.Instance.SetObjectiveAsComplete(itemID, this.gameObject);
        }
    }


    private void OnTriggerExit(Collider collision)
    {
        if (!realItemDetected) return;
        EventHandler.Instance.LogMessage("Collision exit " + type.ToString());

        LiveModelCollision hitModel = collision.gameObject.GetComponent<LiveModelCollision>();
        if (hitModel == null) return;
        if (hitModel.liveModelType == this.type)
        {
            resultPack.succeeded = false;
            meshRenderer.material = defaultMaterial;
            //EventHandler.Instance.OnUtensilObjectiveFailed(this);

            EventHandler.Instance.SetObjectiveAsUncomplete(itemID, this.gameObject);
        }
    }
}
