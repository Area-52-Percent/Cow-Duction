/*  FanRotation.cs

    Constantly rotates an object around the specified axis.
*/

using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public enum rotationAxes { X, Y, Z };
    
    public rotationAxes rotationAxis = rotationAxes.X;
    public float rotationSpeed = 1.0f;

    // Update is called once per frame
    void Update()
    {
        float rotation = Time.deltaTime * rotationSpeed;
        transform.Rotate((rotationAxis == rotationAxes.X) ? rotation : 0, 
                         (rotationAxis == rotationAxes.Y) ? rotation : 0,
                         (rotationAxis == rotationAxes.Z) ? rotation : 0);
    }
}
