using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeObjectiveBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Materials")]
    public Material goalReachedMaterial;
    public Material defaultMaterial;

    [Header("children elements")]
    public List<MeshRenderer> childrenMeshes;

    [Header("Sound effect when objective is reached")]
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;

    private AudioSource audioSource; // we do a seperate sound effect here, the reason being so we can detect when this object already played the sound once, we stop it from playing it again
    private bool soundPlayed = false;

    private bool goalReached = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
    }
    private void OnEnable()
    {
        EventHandler.OnBeforeMatlabDataReceived += ResetColor; 
        //EventHandler.OnObjectiveCompleted += OnGoalReached;
    }

    private void OnDisable()
    {
        EventHandler.OnBeforeMatlabDataReceived -= ResetColor;
        //EventHandler.OnObjectiveCompleted -= OnGoalReached;
    }

    //resets the color to be white before the kinect data is handled
    void ResetColor()
    {
        foreach (MeshRenderer mesh in childrenMeshes)
        {
            mesh.material = defaultMaterial;
        }
        goalReached = false;
    }


    //called when a laser touches the goal. called by the laser when hitting the collider
    public void GoalTouchedByBeam()
    {
        if (goalReached) return;
        goalReached = true;
        //this is to prevent double laser on same goal triggering a gg for multiple goal maps
        if (EventHandler.Instance != null) EventHandler.Instance.SetObjectiveStepAsAchieved(MazeObjectives.INIT_OBJECTIVE_INDEX, this.gameObject);
        foreach (MeshRenderer mesh in childrenMeshes)
        {
            mesh.material = goalReachedMaterial;
        }

        //play sound effect 
        if (!soundPlayed)
        {
            audioSource.Play();
            soundPlayed = true;
        }
    }
}
