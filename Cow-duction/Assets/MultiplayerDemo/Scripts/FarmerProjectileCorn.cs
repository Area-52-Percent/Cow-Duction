using UnityEngine;
using Mirror;

// Copy of Projectile.cs from Mirror.Examples.Tanks
public class FarmerProjectileCorn : FarmerProjectile
{
    // ServerCallback because we don't want a warning if OnTriggerEnter is
    // called on the client
    [ServerCallback]
    void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.tag == "exhaust")
        {
            spaceshipCanvas.TakeDamage(projectileDamage, 'c');
            GameObject milkLeakClone = Instantiate(milkLeak, transform.position, transform.rotation);
            NetworkServer.Spawn(milkLeakClone);
            milkLeakClone.AddComponent<AudioSource>().PlayOneShot(projectileHit, 0.25f);
        }
        else if (co.gameObject.GetComponent<MultiPlayerSpaceshipController>())
        {
            spaceshipCanvas.TakeDamage(projectileDamage, 'c');
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