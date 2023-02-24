using BoingKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * The avatar uses the Boing mouvement asset from the asset store (paying but I already had it so I used it)
 * This class extends the basic controller to set destinations to the avatar for the mouvement
 * The avatar does not have an animator but has a bouncy idle animatio,
 */
public class AvatarMouvement : UFOController
{

    [Header("Avatar destination and behaviour")]
    [SerializeField] private Vector3 defaultLocation;

    float distanceMargin = 0.1f;
    Vector3 Destination = Vector3.zero;

    //sets the destination of the Avatar ! This is a local position relative to the stones origin
    public void SetDestination(Vector3 destination)
    {
        this.Destination = destination;
    }

    public void SetDestination(GameObject target)
    {
        this.Destination = target.transform.localPosition;
    }

    //puts the avatar back to default position
    public void ResetPosition()
    {
        this.Destination = defaultLocation; //we set the location as the default location
    }

    void FixedUpdate()
    {
        if (Destination == Vector3.zero) return;
        Vector3 linearInputVec = Destination - transform.localPosition;
        if (linearInputVec.magnitude <= distanceMargin)
        {
            Destination = Vector3.zero;
            if (EventHandler.Instance != null) EventHandler.Instance.InformAvatarReachedDestination(); //we inform anyone that needs to know that the avatar reached the destination
            return;
        }
        CalculateMouvement(linearInputVec);
    }
}
