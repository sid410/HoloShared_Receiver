using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//for 3D display of text on top of 3D items
public class ItemTextHandler : MonoBehaviour
{

    public TextMesh textDisplay;


    // Update is called once per frame
    public void DisplayText(string text)
    {
        textDisplay.text = text;
    }
}
