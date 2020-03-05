using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FarmerProjectile : NetworkBehaviour
{
    public float destroyAfter = 5;
    public Rigidbody rigidBody;
    public ParticleSystem gunSmoke;
    public float hitForce = 1000;
    public float speedForce = 1000;
    public GameObject owner;

    [Header("Hit Feedback")]
    public GameObject milkLeak;
    public AudioClip projectileHit;
    public float projectileDamage = 5f;

    protected SpaceshipCanvas spaceshipCanvas;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    void Start()
    {
        spaceshipCanvas = SpaceshipCanvas.instance;
        rigidBody.AddForce(transform.forward * speedForce);
    }

    // destroy for everyone on the server
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
