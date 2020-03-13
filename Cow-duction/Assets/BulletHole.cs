using UnityEngine;
using System.Collections;

public class BulletHole : MonoBehaviour
{

    public float despawnTime;

    void Start()
    {
        StartCoroutine(DespawnTime());
    }

    IEnumerator DespawnTime()
    {

        //Wait for x amount of seconds
        yield return new WaitForSeconds(despawnTime);

        //Destroy the bullet hole
        Destroy(gameObject);
    }
}
