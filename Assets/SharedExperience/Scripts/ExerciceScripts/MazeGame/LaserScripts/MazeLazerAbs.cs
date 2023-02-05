using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//abstract class for lasers because it's better for referencing
public abstract class MazeLazerAbs : MonoBehaviour
{
    public abstract void UpdateLaser(float laserRange);
}
