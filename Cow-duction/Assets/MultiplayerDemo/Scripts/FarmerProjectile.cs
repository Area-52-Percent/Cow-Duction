using UnityEngine;
using Mirror;

// Copy of Projectile.cs from Mirror.Examples.Tanks
public class FarmerProjectile : NetworkBehaviour
{
    public float destroyAfter = 5;
    public Rigidbody rigidBody;
    public float force = 1000;

    [Header("Hit Feedback")]
    public GameObject milkLeak;
    public AudioClip projectileHit;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    void Start()
    {
        rigidBody.AddForce(transform.forward * force);
    }

    // destroy for everyone on the server
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    // ServerCallback because we don't want a warning if OnTriggerEnter is
    // called on the client
    [ServerCallback]
    void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.GetComponent<MultiPlayerSpaceshipController>())
        {
            co.gameObject.GetComponent<MultiPlayerSpaceshipController>().AddImpulseForce(rigidBody.velocity.normalized, force * rigidBody.mass);

            GameObject milkLeakClone = Instantiate(milkLeak, transform.position, transform.rotation, co.transform);
            NetworkServer.Spawn(milkLeakClone);
            milkLeakClone.AddComponent<AudioSource>().PlayOneShot(projectileHit, 0.25f);
        }
        NetworkServer.Destroy(gameObject);
    }
}
