using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MazeObstacle;


//handles the line of laser going off and bouncing.
public class MazeLaser
{

    Vector3 spawnPosition, direction;
    GameObject laserObject; //refreshed at each tick
    LineRenderer laser; 
    List<Vector3> laserIndices = new List<Vector3>(); //indices the line renderer goes through

    public MazeLaser(Vector3 spawnPos, Vector3 direction, Material material)
    {
        this.laser = new LineRenderer();
        this.laserObject = new GameObject();
        this.laserObject.name = "L_b";
        this.spawnPosition = spawnPos;
        this.direction = direction;

        this.laser = this.laserObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
        this.laser.startWidth = 0.03f;
        this.laser.endWidth = 0.03f;
        this.laser.material = material;
        this.laser.endColor = Color.green;
        this.laser.startColor = Color.green;
        this.laser.numCornerVertices = 6;
        this.laser.numCapVertices = 6;

        CastRay(spawnPos, direction, laser);
    }


    void CastRay(Vector3 pos, Vector3 dir, LineRenderer laser)
    {
        laserIndices.Add(pos);

        Ray ray = new Ray(pos, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30, 1))
        {
            CheckHit(hit, dir, laser);
        } else
        {
            laserIndices.Add(ray.GetPoint(30));

        }
        UpdateLaser();
    }


    void UpdateLaser()
    {
        int count = 0;
        laser.positionCount = laserIndices.Count;

        foreach (Vector3 idx in laserIndices)
        {
            laser.SetPosition(count, idx);
            count++;
        }
    }

    void CheckHit(RaycastHit hitInfo, Vector3 direction, LineRenderer laser)
    {
        MazeObstacle obstacleType = hitInfo.collider.gameObject.GetComponent<MazeObstacle>();

        //when hitting an unexpected item, ignore
        if (obstacleType == null)
        {
            laserIndices.Add(hitInfo.point);
            UpdateLaser(); //TODO : maybe ignore
            return;
        }


        switch (obstacleType.type)
        {
            case MazeObstacleType.MIRROR:
                Vector3 pos = hitInfo.point;
                Vector3 dir = Vector3.Reflect(direction, hitInfo.normal);
                CastRay(pos, dir, laser);
                break;

            case MazeObstacleType.WALL:
                laserIndices.Add(hitInfo.point);
                UpdateLaser();
                break;

            case MazeObstacleType.GOAL:
                EventHandler.Instance.SetObjectiveAsComplete(MazeObjectives.INIT_OBJECTIVE_INDEX, null);
                break;
        }

    }
    public GameObject getSpawnedObj() { return laserObject; }
}
