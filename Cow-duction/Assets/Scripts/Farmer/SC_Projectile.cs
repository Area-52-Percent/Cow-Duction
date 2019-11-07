/*  SC_Projectile.cs

    On collision with an object, destroy this and if it is the UFO, deal damage.

    Assumptions:
        There is a GameObject in the scene named "UFO".
        There is a GameObject in the scene with the "UIManager" tag and SC_AlienUIManager component.
        Layer 12 is "Farmer" and layer 13 is "UFO".
 */

using System.Collections;
using UnityEngine;

public class SC_Projectile : MonoBehaviour
{
    [SerializeField] private SC_AlienUIManager uIManager;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject milkLeak;
    [SerializeField] private float projectileDamage = 5.0f;

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
                uIManager.TakeDamage(projectileDamage);
                GameObject milkLeakClone = Instantiate(milkLeak, collision.GetContact(0).point, Quaternion.identity);
                milkLeakClone.transform.parent = targetObject.transform;
            }
        }
        if (collision.gameObject.layer != 12) // 12 = Farmer
            Destroy(this.gameObject);
    }    
}
