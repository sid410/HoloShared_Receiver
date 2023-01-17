using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeUtensil : UtensilAbs
{


    private void Start()
    {
        EventHandler.Instance.OnNewItemSpawned(this.gameObject);
    }

    //we reposition according to the table
    public override void RepositionRealGameobject(Vector3 tablePos, float tableRot)
    {
        EventHandler.Instance.OnLog("Repositioning real gameobject ! " + gameObject.name);
        gameObject.transform.localPosition = tablePos;
        gameObject.transform.localEulerAngles = new Vector3(0, tableRot, 0);
    }
}
