/*  DestroySelf.cs

    Kills the GameObject after a set period of time.
 */

using System.Collections;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3.0f;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(Die());
    }

    // Kill self
    private IEnumerator Die()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(this.gameObject);
    }
}
