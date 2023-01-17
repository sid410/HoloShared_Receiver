using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * Preset for spawning Stones 
 */
[CreateAssetMenu(fileName = "Assets/Resources/Data/ExerciseData/ItemSpawn/spawnPreset.asset", menuName = "ScriptableObjects/Create spawn preset", order = 5)]

public class ItemsSpawnEntry : ScriptableObject
{
    [Serializable]
    public class SpawnPoint
    {
        public GameObject itemPrefab;
        [Header("Local position in relation to the stones target")]
        [Range(0f, 1f)]
        public float PosX;
        [Range(0f, 1f)]
        public float PosY;

        public Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 rotation;

        public string message = null; //used for tutorial for now, spawns a billboard with a message on top of the spawned item
    }

    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
}
