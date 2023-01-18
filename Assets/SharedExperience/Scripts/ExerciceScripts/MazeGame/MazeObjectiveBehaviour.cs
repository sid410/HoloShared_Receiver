using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeObjectiveBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public Material goalReachedMaterial;
    public List<MeshRenderer> childrenMeshes;

    private void OnEnable()
    {
        EventHandler.OnObjectiveCompleted += OnGoalReached;
    }

    private void OnDisable()
    {
        EventHandler.OnObjectiveCompleted -= OnGoalReached;
    }

    void OnGoalReached(int objectiveCode, GameObject caller)
    {
        if (objectiveCode != MazeObjectives.INIT_OBJECTIVE_INDEX) return;
        foreach (MeshRenderer mesh in childrenMeshes)
        {
            mesh.material = goalReachedMaterial;
        }
    }
}
