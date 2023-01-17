using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/** 
 * Handles spawning items for both exercise and Tutorial steps, also handles displaying any related message on top of them if eneded 
 */
public class ExerciseItemInstantiater : MonoBehaviour
{
    #region calibration
    private float Xcalibration = -0.02f; //negligeable ignore;
    private float rotationCalibrationY = 30f; //changed by the calibration result of the stone image, so exercise maps spawned are correctly oriented on the table
    #endregion

    [SerializeField] private GameObject stonesOrigin;
    [SerializeField] private ItemTextHandler item3DTextPrefab;

    private Queue<GameObject> spawnedItems = new Queue<GameObject>();

    private void OnEnable()
    {
        EventHandler.calibrationDone += SaveCalibrationData;
        EventHandler.Tutorial_step += SpawnTutorialItems;
        EventHandler.Exercice_step += SpawnExerciseItems; //we spawn items for exercises
        EventHandler.ResetApp += DeleteSpawnedItems;
    }

    private void OnDisable()
    {
        EventHandler.calibrationDone -= SaveCalibrationData;
        EventHandler.Tutorial_step -= SpawnTutorialItems;
        EventHandler.Exercice_step -= SpawnExerciseItems; //we spawn items for exercises
        EventHandler.ResetApp -= DeleteSpawnedItems;
    }

    private void SaveCalibrationData(Vector3 positionOffset, Quaternion rotation) => rotationCalibrationY = rotation.eulerAngles.y;

    private void SpawnTutorialItems(TutorialData.TutorialStep ts) => SpawnStepItems(ts.tutorialitems);

    private void SpawnExerciseItems(ExerciceData.ExerciceStep es) => SpawnStepItems(es.spawnEntry);

    void SpawnStepItems(ItemsSpawnEntry spawnEntrys)
    {
        //For the purpose of exercises, it is preferable to use full maps, since its better for rotation everything on the table with the same rotation vector
        //For maps and for Flexibility
        DeleteSpawnedItems(); //we remove previous items
        foreach (ItemsSpawnEntry.SpawnPoint spawn in spawnEntrys.spawnPoints)
        {
            //we instantiate the prefab, set the stonesOrigin as parent and fix all rotations/transform problems
            GameObject spawnedObject = Instantiate(spawn.itemPrefab);
            spawnedObject.transform.parent = stonesOrigin.transform;
            spawnedObject.transform.localPosition = new Vector3(spawn.PosX + Xcalibration, 0, -spawn.PosY);
            if (!spawn.scale.Equals(Vector3.zero)) spawnedObject.transform.localScale = spawn.scale;

            Vector3 rotation = (spawn.rotation.Equals(Vector3.zero)) ? spawn.itemPrefab.transform.rotation.eulerAngles : spawn.rotation;
            spawnedObject.transform.rotation = Quaternion.Euler(rotation + new Vector3(0, rotationCalibrationY, 0));

            if (spawn.message != null) //if the message is not null, we spawn a billboard on top the item and set the text
            {
                ItemTextHandler itemText = Instantiate(item3DTextPrefab, spawnedObject.transform);

                //now we position the text, if the gameobject has a direct child that's name is "TextPosition", it is used to position the Text 3D display. Otherwise we put a default position of 0.1f on the Y
                GameObject textSpawnPoint = spawnedObject.transform.Find("TextPosition").gameObject; 
                itemText.transform.localPosition = (textSpawnPoint == null) ? new Vector3(0, 0.1f, 0) : textSpawnPoint.transform.localPosition;
                itemText.DisplayText(spawn.message); //we finally set the text.
            }
        }
    }


    void DeleteSpawnedItems()
    {
        while (spawnedItems.Count > 0)
        {
            GameObject item = spawnedItems.Dequeue();
            Destroy(item);
        }
    }
}
