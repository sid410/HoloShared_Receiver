using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The game works in a single exercise step, which is to bring waves after waves and occasionally spawn boosses
/// </summary>
public class SnowGameManager : MonoBehaviour
{

    public static SnowGameManager Instance { private set; get; }

    [Header("Settings for the game")]
    [SerializeField] private SnowballDifficultyParamters parameters;

    [Header("Prefabs")]
    [SerializeField] private Damageable soldierPrefab;

    [Header("Positions (local)")]
    [SerializeField] private float allySpawnPosition;
    [SerializeField] private float enemySpawnPosition;

    [Header("territory boundaries (local)")]
    [SerializeField] private float allyTerritoryLimit;
    [SerializeField] private float enemyTerritoryLimit;

    //we get the origin of the stones calibration
    private GameObject StonesOrigin;

    //we keep track of soldiers
    private List<Damageable> SpawnedEnemySoldiers = new List<Damageable>();
    private List<Damageable> SpawnedAllySoliders = new List<Damageable>();

    //we keep track of structures
    private List<GameObject> enemyStructures = new List<GameObject>();
    private List<GameObject> allyStructures = new List<GameObject>();

    public SnowballDifficultyParamters gameParameters;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        //StartGame();
    }
    private void StartGame()
    {
        //we start by spawning the ally soldiers
        int allySoliders = parameters.allySoldiers;

        for (int i = 0; i < allySoliders; i++)
        {
            Damageable soldier = Instantiate(soldierPrefab, StonesOrigin.transform);
            soldier.Init(Team.ALLY, new Vector3(-0.28f, -0.98f, allySpawnPosition)); //TODO: fix
            SpawnedAllySoliders.Add(soldier);
        }
    }

    public float getEnemyTerritoryLimit() { return enemyTerritoryLimit; }
}
