using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MazeObstacle;


//handles the line of laser going off and bouncing.
public class MazeLaser
{

    Vector3 spawnPosition, direction;
    GameObject laserObject; //refreshed at each tick
    Material material;
    LineRenderer laser; 
    List<Vector3> laserIndices = new List<Vector3>(); //indices the line renderer goes through

    GameObject lasthitObject = null; //last object hit by ray is ignored for the next raycast

    List<MazeLaser> SplitLasers = new List<MazeLaser>(); //lasers can split up

    private float laserCastRange = 5; //raycast maximum distance
    public MazeLaser(Vector3 spawnPos, Vector3 direction, Material material)
    {
        InitializeLaser(spawnPos, direction, material);
    }


    //for split lasers (triggered by refactored light from bottle/cup)
    public MazeLaser(Vector3 spawnPos, Vector3 direction, Material material, float maxRange, GameObject ignoredObject)
    {
        //this.SetIgnoredObject(ignoredObject);
        lasthitObject = ignoredObject; //we update the last hit object to the new one.
        laserCastRange = maxRange;
        InitializeLaser(spawnPos, direction, material);
    }

    //used for testing with lower range
    public MazeLaser(Vector3 spawnPos, Vector3 direction, Material material, float maxRange)
    {
        laserCastRange = maxRange;
        InitializeLaser(spawnPos, direction, material);
    }

    //because c# constructors SUCK
    void InitializeLaser(Vector3 spawnPos, Vector3 direction, Material material)
    {
        this.laser = new LineRenderer();
        this.laserObject = new GameObject();
        this.material = material;
        this.laserObject.name = "L_b";
        this.spawnPosition = spawnPos;
        this.direction = direction;

        this.laser = this.laserObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
        this.laser.startWidth = 0.02f;
        this.laser.endWidth = 0.02f;
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
        if (lasthitObject != null) lasthitObject.GetComponent<Collider>().enabled = false; //we remove the collider from last hit object to prevent being stuck inside a collider for any reason
        if (Physics.Raycast(ray, out hit, laserCastRange, 1))
        {
            if (lasthitObject != null) lasthitObject.GetComponent<Collider>().enabled = true; //we reenable the colldier as soon as the raycast ends
            CheckHit(hit, dir, laser);
        } else
        {
            if (lasthitObject != null) lasthitObject.GetComponent<Collider>().enabled = true;
            laserIndices.Add(ray.GetPoint(laserCastRange));

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

        lasthitObject = hitInfo.collider.gameObject; //we update the last hit object to the new one.
        //SetIgnoredObject(hitInfo.collider.gameObject); //we update the ignored object
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

            case MazeObstacleType.REFRACTOR:
                Vector3 posi = hitInfo.point;
                Vector3 newDirection = Quaternion.Euler(0, -30, 0) * direction;
                SplitLasers.Add(new MazeLaser(hitInfo.point, Quaternion.Euler(0, 30, 0) * direction, material, laserCastRange, lasthitObject)); //we split off a second laser. we pass the current hit object to not cause insta collision.
                CastRay(posi, newDirection, laser);
                break;

            case MazeObstacleType.GOAL:
                laserIndices.Add(hitInfo.point); //we add the point to the linerender path (it stops at the goal)

                //we tell the maze goal that it was hit, making it change color
                MazeObjectiveBehaviour mazeObjectiveBehaviour = hitInfo.collider.gameObject.GetComponent<MazeObjectiveBehaviour>();
                if (mazeObjectiveBehaviour != null) mazeObjectiveBehaviour.GoalTouchedByBeam();
                CastRay(hitInfo.point, direction, laser); //goal doesn't stop ray
                //UpdateLaser(); <
                break;

        }

    }

    public GameObject Cleanup() 
    { 
        if (lasthitObject != null) lasthitObject.GetComponent<Collider>().enabled = true;
        foreach(MazeLaser laser in SplitLasers) //we cleanup split lasers
        {
            GameObject.Destroy(laser.Cleanup());
        }
        return laserObject; 
    }
}
