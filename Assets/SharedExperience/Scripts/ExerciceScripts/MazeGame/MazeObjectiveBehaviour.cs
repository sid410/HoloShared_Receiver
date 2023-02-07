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
    }


    //called when a laser touches the goal. called by the laser when hitting the collider
    public void GoalTouchedByBeam()
    {
        //this is to prevent double laser on same goal triggering a gg for multiple goal maps
        if (EventHandler.Instance != null) EventHandler.Instance.SetObjectiveStepAsAchieved(MazeObjectives.INIT_OBJECTIVE_INDEX, this.gameObject);
        foreach (MeshRenderer mesh in childrenMeshes)
        {
            mesh.material = goalReachedMaterial;
        }
    }
}
