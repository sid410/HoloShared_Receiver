using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Debug class, displays logs. Disabled if no debug mode.
 */
public class LogDisplayer : MonoBehaviour
{
    public static LogDisplayer instance { get; private set; }

    [SerializeField] private GameObject logBillboard;
    [SerializeField] private TextMesh logTexts;


    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        if (!LocalItemSpawner.SINGLE_HOLO_BEHAVIOUR) //if we aren't debugging we disable the billboard and do not list the logs. Maybe for future if logs are saved to a text file unblock this and do the saves
        {
            logBillboard.SetActive(false);
            return;
        }
        EventHandler.OnLog += UpdatelogTexts;
        EventHandler.OnExerciseStarted += (ed) => UpdatelogTexts("Exercice started");
        EventHandler.OnExerciseOver += () => UpdatelogTexts("Exercice Over");
    }

    public void UpdatelogTexts(string message)
    {
        logTexts.text += "\n" + message;
        Debug.Log(message);
    }
}
