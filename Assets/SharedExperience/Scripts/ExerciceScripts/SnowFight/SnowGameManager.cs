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

    [Header("Positions")]
    [SerializeField] private Transform allySpawnPosition;
    [SerializeField] private Transform enemySpawnPosition;

    //we keep track of soldiers
    private List<Damageable> SpawnedEnemySoldiers = new List<Damageable>();
    private List<Damageable> SpawnedAllySoliders = new List<Damageable>();

    public SnowballDifficultyParamters gameParameters;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        StartGame();
    }
    private void StartGame()
    {
        //we start by spawning the ally soldiers
        int allySoliders = parameters.allySoldiers;

        for (int i = 0; i < allySoliders; i++)
        {
            Damageable soldier = Instantiate(soldierPrefab);
            soldier.Init(Team.ALLY, allySpawnPosition.position);
            SpawnedAllySoliders.Add(soldier);
        }
    }
}
