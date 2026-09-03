using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine References")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    public void ShakeCamera(float shakeForce)
    {
        if (impulseSource == null)
        {
            return;
        }
        shakeForce = Mathf.Max(shakeForce, 0f);
        impulseSource.GenerateImpulseWithForce(shakeForce);
    }
}