using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowfightUtil : MonoBehaviour
{
    private static int allyLayer = LayerMask.NameToLayer("ally");
    private static int enemyLayer = LayerMask.NameToLayer("enemy");

    public const string STRUCTURE_TAG = "structure";
    public const string ENTITY_TAG = "entity";
    //gets the layer the object should be in depending on the layer
    public static int getTeamLayer(Team team)
    {
        return team == Team.ALLY ? allyLayer : enemyLayer;
    }

    public static int getAllyLayerMask() { return allyLayer; }

    public static int getEnemyLayerMask() { return enemyLayer; }
}
