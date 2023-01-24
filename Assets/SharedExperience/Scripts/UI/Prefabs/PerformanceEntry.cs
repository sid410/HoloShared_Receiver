using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a line of performance for score calculation, used for the prefab.
public class PerformanceEntry : MonoBehaviour
{
    [SerializeField] TextMesh performance_name;
    [SerializeField] TextMesh performance_value;

    public void UpdateUI(string pName, string pValue)
    {
        performance_name.text = pName;
        performance_value.text = pValue;
    }
}
