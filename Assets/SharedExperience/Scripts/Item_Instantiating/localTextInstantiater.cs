using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
///  attached to items, handles instantiating an info billboard locally, this is an alternative to the ExerciseItemInstantiater manually Instantiating the text if needed
///  Don't use this on prefabs, add this to a game map's objects but not to the root prefabs !
/// </summary>
public class localTextInstantiater : MonoBehaviour
{

    public ItemTextHandler billboardPrefab;
    public string text;

    private void Start()
    {
        if (billboardPrefab == null || text == null) return;
        ItemTextHandler itemText = Instantiate(billboardPrefab, gameObject.transform);

        //now we position the text, if the gameobject has a direct child that's name is "TextPosition", it is used to position the Text 3D display. Otherwise we put a default position of 0.1f on the Y
        GameObject textSpawnPoint = transform.Find("TextPosition").gameObject;
        itemText.transform.localPosition = (textSpawnPoint == null) ? new Vector3(0, 0.1f, 0) : textSpawnPoint.transform.localPosition;
        itemText.DisplayText(text); //we finally set the text.
    }
}
