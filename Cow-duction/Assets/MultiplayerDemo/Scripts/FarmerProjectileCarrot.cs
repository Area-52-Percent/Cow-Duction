using UnityEngine;
using Mirror;

// Copy of Projectile.cs from Mirror.Examples.Tanks
public class FarmerProjectileCarrot : NetworkBehaviour
{
    public float destroyAfter = 5;
    public Rigidbody rigidBody;
    public float hitForce = 1000;
    public float speedForce = 1000;
    public GameObject owner;

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
        rigidBody.AddForce(transform.forward * speedForce);
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
            co.gameObject.GetComponent<MultiPlayerSpaceshipController>().AddImpulseForce(rigidBody.velocity.normalized, hitForce * rigidBody.mass);

            GameObject milkLeakClone = Instantiate(milkLeak, transform.position, transform.rotation);
            NetworkServer.Spawn(milkLeakClone);
            milkLeakClone.AddComponent<AudioSource>().PlayOneShot(projectileHit, 0.25f);
        }
        else if (co.gameObject.GetComponent<AmmoSack>())
        {
            co.gameObject.GetComponent<AmmoSack>().AmmoSwap(owner);
        }
        NetworkServer.Destroy(gameObject);
    }
}