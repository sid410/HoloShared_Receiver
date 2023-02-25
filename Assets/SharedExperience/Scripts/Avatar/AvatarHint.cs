using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Hint ! Added to an empty gameobject and placed at position you want the avatar to move to.
 * Avatar collects all these gameobjects when the map is loaded and gives hints from time to time
 */
public class AvatarHint : MonoBehaviour
{
    [Header("Parnet of this object must be named \"Hints\". Used in AvatarMouvement.cs ")]
    [TextArea(3,3)]
    public string message;
    public float countdown;


    //TODO : way to trigger a condition

}
