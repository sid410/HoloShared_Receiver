using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtensilExtension;

public class UtensilKinectHandler : KinectResultsAbs
{
    List<GameObject> spawnedItems = new List<GameObject>();
    public override void Init() //initializes the data by registering to events
    {
        EventHandler.NewItemSpawned += (item) => spawnedItems.Add(item);
    }

    public override void HandleKinectData(List<localKinectReceiver.KinectUtensilData> kinectResults)
    {
        List<GameObject> virtualObjectsList = new List<GameObject>(spawnedItems); //we copy the list of items
        foreach (localKinectReceiver.KinectUtensilData kinectResult in kinectResults)
        {
            if (kinectResult.type == UtensilType.UNDETECTED) continue; //we ignore undetected items
            GameObject virtualGO = FindLeastDistanceSameObject(kinectResult.position, spawnedItems, kinectResult.type); //we find the cloesest item
            SetRealOriginAndAxis(virtualGO, kinectResult.position, kinectResult.orientation + (kinectResult.type == UtensilType.SPOON ? 180 : 0)); //we pass the data to the related script
            virtualObjectsList.Remove(virtualGO); //we remove from our cloned list so we don't contact the same utensil twice
        }
        
    }

    private GameObject FindLeastDistanceSameObject(Vector3 realOrigin, List<GameObject> virtualObjects, UtensilType utensilType)
    {
        Dictionary<GameObject, float> sameTagObjects = new Dictionary<GameObject, float>();

        foreach (GameObject gObject in virtualObjects)
        {
            UtensilAbs utensilData = gObject.GetComponent<UtensilAbs>();
            if (utensilData == null) continue;
            if (utensilData.type != utensilType) continue;

            float distance = Vector3.Distance(gObject.transform.position, realOrigin);
            sameTagObjects.Add(gObject, distance);
        }

        if (sameTagObjects.Count == 0) return null;

        GameObject nearest = sameTagObjects.FirstOrDefault(x => x.Value == sameTagObjects.Values.Min()).Key;
        return nearest;
    }


    //uses data received from the kinect to repositino the position of the real object tied to the virtual one
    private void SetRealOriginAndAxis(GameObject virtualObject, Vector3 tablePos, float tableRot)
    {
        if (virtualObject == null) return;
        UtensilBehaviour utensilBehaviour = virtualObject.GetComponent<UtensilBehaviour>();

        if (utensilBehaviour == null) return;
        utensilBehaviour.RepositionRealGameobject(tablePos, tableRot);
    }


}
