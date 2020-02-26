using UnityEngine;

[ExecuteInEditMode]
public class WindmillEditor : MonoBehaviour
{
    [Header("Parameters")]
    [Range(0.1f, 10)]
    public float fanScale = 1;
    [Range(0.1f, 10)]
    public float shaftScale = 1;
    [Range(0, 360)]
    public float motorRotation = 0;
    public float fanSpeed = 30;

    [Header("Relative Factors")]
    public float colliderCenterFactor = 20f;
    public float colliderLengthFactor = 50f;
    public float particleRadiusFactor = 10f;
    public float motorPositionFactor = 35f;
    public float speedPushFactor = 0.5f;

    [Header("Objects")]
    [SerializeField] private Transform fan;
    [SerializeField] private Transform motor;
    [SerializeField] private Transform shaft;
    [SerializeField] private GameObject windZone;
    [SerializeField] private ParticleSystem windParticles;

    private ConstantRotation fanRotation;
    private EnvironmentWind environmentWind;
    private CapsuleCollider windZoneCollider;

    void OnValidate()
    {
        fan.localScale = new Vector3(fanScale, 1, fanScale);

        if (fanRotation == null) fanRotation = fan.GetComponent<ConstantRotation>();
        fanRotation.rotationSpeed = fanSpeed;

        if (environmentWind == null) environmentWind = windZone.GetComponent<EnvironmentWind>();
        environmentWind.pushForce = fanSpeed * speedPushFactor;

        if (windZoneCollider == null) windZoneCollider = windZone.GetComponent<CapsuleCollider>();
        windZoneCollider.center = Vector3.forward * fan.localScale.x * colliderCenterFactor;
        windZoneCollider.height = fan.localScale.x * colliderLengthFactor;

        var particleShape = windParticles.shape;
        particleShape.radius = fan.localScale.x * particleRadiusFactor;

        motor.localEulerAngles = Vector3.up * motorRotation;

        shaft.localScale = new Vector3(1, shaftScale, 1);
        motor.localPosition = Vector3.up * shaft.localScale.y * motorPositionFactor;
    }
}
