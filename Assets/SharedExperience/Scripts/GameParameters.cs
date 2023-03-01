using M2MqttUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Receives and saves any in game options that can affect the game.
public class GameParameters : MonoBehaviour
{

    public static GameParameters instance { private set; get; }
    public BaseClient baseClient;


    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
