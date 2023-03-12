using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class SoldierBehaviour : Damageable
{

    public enum SoldierState
    {
        KNOCKED, EXPOSED, HIDDEN
    }

    [Header("Soldier data, for attacking")]
    [SerializeField] protected Projectile projectilePrefab;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float windupTime = 3f;
    [SerializeField] protected float coneRangeWideness = 0.3f; //how far on x axis can enemy be to be targetable
    [SerializeField] protected float destinationDistanceMargin = 0.02f;
    [SerializeField] protected Transform projectileSpawnPosition;

    [Header("Attack pattern")]
    [SerializeField] protected float halfBoxTarget = 0.2f;

    [Header("UI Elements")]
    [SerializeField] protected Image windupCircle;

    

    //attack
    protected float currentWindupTimer = 0f;
    //mouvement
    protected bool isMoving = false; //if moving, no attacking
    protected bool isGarrisoned = false;
    protected Vector3 destination = Vector3.zero;
    protected SnowStructure allyStructure = null; //structure behind which the soldier will or is garrisonning

    protected int enemyLayer = 1;

    public SoldierState currentState { protected set; get; } //current state in case of attack

    protected virtual void Start()
    {
        //Look for ally structure
        currentState = SoldierState.EXPOSED;
        isMoving = true;
        isGarrisoned = false;
        destination = Vector3.zero;

        //if no structure is found go straight
        InitialMouvement();
    }

    protected abstract void InitialMouvement();

    protected virtual void Update()
    {
        MoveToDestination(); //mouvements
        PrepareAttack(); //attacking when not moving


    }

    protected void MoveToDestination()
    {
        if (destination == Vector3.zero) return;
        Vector3 direction = destination - transform.position;
        transform.position = transform.position + direction * Time.deltaTime * moveSpeed;
        if (direction.magnitude <= destinationDistanceMargin)
        {
            isMoving = false;

            //if we reached a structure, we garrison the soldier
            if (allyStructure != null)
            {
                currentState = SoldierState.HIDDEN; //TODO : check if it was actually full or not !!!
                allyStructure.GarrisonSolider(this);
            }
        }
    }
    //prepares the attack windup and triggers it when duration is over
    private void PrepareAttack()
    {
        if (isMoving) return;
        currentWindupTimer += Time.deltaTime;
        windupCircle.fillAmount = (currentWindupTimer / windupTime);
        if (currentWindupTimer >= windupTime)
        {
            currentWindupTimer = 0f;
            TriggerAttack();
        }

    }


    //triggers an attack, a projectile is launched in predefined direction
    protected void TriggerAttack()
    {
        currentWindupTimer = 0f;


        Damageable target = getTargetUnit();

        Projectile attackProjectile = Instantiate(projectilePrefab, projectileSpawnPosition.position, Quaternion.identity);
        attackProjectile.Init(this.team, target);

    }

    //implemented by child functions, returns the targeted unit by the attacks
    protected virtual Damageable getTargetUnit() 
    {
        List<RaycastHit> targetsInRange = Physics.BoxCastAll(transform.position, new Vector3(halfBoxTarget, coneRangeWideness, 2f), transform.forward, Quaternion.identity, 2f, enemyLayer).ToList();

        float soliderZposition = transform.position.z;
        targetsInRange.Sort((t1, t2) => t1.transform.position.z.CompareTo(t2.transform.position.z)); //we sort by distance, we start going one by one until we hit a real block

        foreach (RaycastHit targetinRange in targetsInRange)
        {
            Damageable targetObj = targetinRange.transform.gameObject.GetComponent<Damageable>();
            if (targetObj == null) continue;

            if (targetObj.unitType == DamageableType.STRUCTURE)
            {
                SoldierBehaviour targetSoldier = ((SnowStructure)targetObj).getGarrisonnedSoldier();
                if (targetSoldier == null) continue; //if no soldier is hiding behind the structure, we check the next target in line
                return targetSoldier; //we return the hidden soldier otherwise
            }
            else
            {
                //case we found a soldier or the castle (the user which should be max distance)
                return targetObj;
            }

        } //TODO : register the castle for ease of access in case of raycast miss
        return null;
    }


    #region Mouvement setters (for changing mouvement)
    protected void StartMoving()
    {
        isMoving = true;
        currentWindupTimer = 0f;
    }

    protected void EndMoving()
    {
        isMoving = false;
        currentWindupTimer = 0f;
    }

    #endregion

}
