/*  SC_Projectile.cs

    On collision with an object, destroy this and if it is the UFO, deal damage.

    Assumptions:
        There is a GameObject in the scene named "UFO".
        There is a GameObject in the scene with the "UIManager" tag and SC_AlienUIManager component.
        Layer 12 is "Farmer" and layer 13 is "UFO".
 */

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SC_Projectile : MonoBehaviour
{
    // Private variables
    private SC_AlienUIManager uIManager;
    private GameObject targetObject;

    // Serialized private variables
    [SerializeField] private GameObject milkLeak = null; // Set up in inspector    
    [SerializeField] private AudioClip projectileHit = null; // Set up in inspector
    [Space]
    [SerializeField] private float projectileDamage = 5.0f;
    [SerializeField] private float knockbackForce = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        uIManager = GameObject.FindWithTag("UIManager").GetComponent<SC_AlienUIManager>();
        targetObject = GameObject.Find("UFO");
    }    

    // Destroy projectile on collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 13) // 13 = UFO
        {
            if (uIManager)
            {
                targetObject.GetComponent<SC_SpaceshipMovement>().AddImpulseForce(GetComponent<Rigidbody>().velocity.normalized, knockbackForce);
                uIManager.TakeDamage(projectileDamage);
                
                GameObject milkLeakClone = Instantiate(milkLeak, collision.GetContact(0).point, Quaternion.identity);
                milkLeakClone.transform.parent = targetObject.transform;      
                milkLeakClone.AddComponent<AudioSource>().PlayOneShot(projectileHit, 0.25f);

                Destroy(gameObject);
            }
        }
    }
}
