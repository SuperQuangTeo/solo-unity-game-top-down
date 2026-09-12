using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine References")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Room Camera")]
    [SerializeField] private Transform roomCameraTarget;

    public void ShakeCamera(float shakeForce)
    {
        if (impulseSource == null)
        {
            return;
        }
        shakeForce = Mathf.Max(shakeForce, 0f);
        impulseSource.GenerateImpulseWithForce(shakeForce);
    }

    public void SetCameraPosition(Vector2 worldPosition)
    {
        if (roomCameraTarget == null)
        {
            return;
        }

        Vector3 newCameraTargetPosition = new Vector3(
            worldPosition.x,
            worldPosition.y,
            roomCameraTarget.position.z
        );

        roomCameraTarget.position = newCameraTargetPosition;
    }

    public void FocusRoom(RoomContext roomContext)
    {
        if (roomContext == null)
        {
            return;
        }

        Transform cameraAnchor = roomContext.CameraAnchor;

        if (cameraAnchor == null)
        {
            return;
        }

        SetCameraPosition(cameraAnchor.position);
    }

}