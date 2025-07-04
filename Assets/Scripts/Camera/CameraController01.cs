using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController01 : MonoBehaviour
{
    [SerializeField]
    public CinemachineFreeLook freeLookCamera;
    [SerializeField]
    public InputReader inputReader;

    [SerializeField]
    public Camera01Setting cameraSetting;

    void Start()
    {
        inputReader.RightStick += Turn;
    }

    void OnDestroy()
    {
        inputReader.RightStick -= Turn;
    }

    void Turn(Vector2 xy, InputActionPhase phase)
    {
        float x = xy.x * Time.deltaTime * cameraSetting.X_Speed;
        float y = xy.y * Time.deltaTime * cameraSetting.Y_Speed;
        freeLookCamera.m_XAxis.m_InputAxisValue = x;
        freeLookCamera.m_YAxis.m_InputAxisValue = y;
    }

    public void SetCameraFollowTarget(Transform target)
    {
        freeLookCamera.Follow = target;
        freeLookCamera.LookAt = target;
    }
}