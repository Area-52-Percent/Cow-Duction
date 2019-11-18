/*  DontDestroy.cs

    Prevents an object from being destroyed on scene change.

    Assumptions:
        The GameObject has a unique tag in the scene.
*/

using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(this.tag);

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        
        DontDestroyOnLoad(this.gameObject);
    }
}
