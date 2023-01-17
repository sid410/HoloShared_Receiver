using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtensilExtension;


//abstract that must be implemented by all gameobject that need informations from the kinect 
public abstract class UtensilAbs : MonoBehaviour
{
    public UtensilType type;


    //uses data received from the kinect to reposition the Gameobject tied to this
    public abstract void RepositionRealGameobject(Vector3 tablePos, float tableRot); //used to communicate the rotation and position of the object

}
