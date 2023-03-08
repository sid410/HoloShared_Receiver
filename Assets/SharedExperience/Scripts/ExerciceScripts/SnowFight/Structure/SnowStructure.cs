using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//script for structures, helps the units coming to know where to position
public class SnowStructure : Damageable
{

    [Header("How much the structure can handle")]
    [SerializeField] private int capacity = 5;

    private int availableSpace = 0;

    private List<SoldierBehaviour> garrisonedSoldiers = new List<SoldierBehaviour>(); //we keep track of soldiers.
    void Start()
    {
        availableSpace = capacity;
    }


    //returns a random soldier hiding behind the structure
    public SoldierBehaviour getGarrisonnedSoldier()
    {
        if (garrisonedSoldiers.Count == 0) return null;
        int randomIndex = Random.Range(0, garrisonedSoldiers.Count); //we return a random soldier
        return garrisonedSoldiers[randomIndex];
    }

    #region soldier Garrisoning
    //called when a soldier wants to hide in this defensive structure
    //returns where the soldier should go to.
    public virtual Vector3 PositionNewSoldier()
    {
        if (availableSpace == 0) return Vector3.zero;
        availableSpace--; //we remove a space
        return transform.position;
    }


    //called when soldier is Dead (in game of course)
    public void UngarrisonSoldier(SoldierBehaviour soldier) 
    {   
        availableSpace++;
        garrisonedSoldiers.Remove(soldier);
    }

    //Called when soldier reaches the location of the structure, the soldier is barricaded inside
    public void GarrisonSolider(SoldierBehaviour soldier)
    {
        Debug.Log("soldier garrisonned");
        garrisonedSoldiers.Add(soldier);

        
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
