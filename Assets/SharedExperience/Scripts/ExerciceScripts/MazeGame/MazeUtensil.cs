using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeUtensil : UtensilAbs
{


    private void Start()
    {
        EventHandler.Instance.SpawnItem(this.gameObject);
    }

    //we reposition according to the table
    public override void RepositionRealGameobject(Vector3 tablePos, float tableRot)
    {
        EventHandler.Instance.LogMessage("Repositioning real gameobject ! " + gameObject.name);
        gameObject.transform.localPosition = tablePos;
        if (tableRot != 0) gameObject.transform.localEulerAngles = new Vector3(0, tableRot, 0);
    }
}
