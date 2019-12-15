using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ContinuousController : MonoBehaviour
{
    public float m_Gravity = 80*9.81f;
    public float m_Sensitivity = 0.5f;
    public float m_MaxSpeed = 1000.0f;
    public float m_RotateIncrement = 90;

    public SteamVR_Action_Boolean m_snapLeftAction = SteamVR_Input.GetBooleanAction("SnapTurnLeft");
    public SteamVR_Action_Boolean m_snapRightAction = SteamVR_Input.GetBooleanAction("SnapTurnRight");
    public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;

    float m_Speed = 0.0f;
    CharacterController m_characterController = null;
    Transform m_CameraRig = null;
    Transform m_Head = null;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_characterController.detectCollisions = false;
    }

    void Start()
    {
        Physics.IgnoreLayerCollision(8, 9);
        m_CameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
    }

    // Update is called once per frame
    void Update()
    {
        HandleHeight();
        CalculateMovement();
        SnapRotation();
    }
    void HandleHeight()
    {
        float headHeight = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_characterController.height = headHeight;

        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_characterController.height / 2;
        newCenter.y += m_characterController.skinWidth;

        // Move capsule in local space
        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;
        m_characterController.center = newCenter;
    }

    void CalculateMovement()
    {
        Quaternion orientation = CalculateOrientation();
        Vector3 movement = Vector3.zero;

        //if not moving
        if (m_MoveValue.axis.sqrMagnitude == 0)
            m_Speed = 0;

        m_Speed += m_MoveValue.axis.magnitude * m_Sensitivity;
        m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);


        movement += orientation * (m_Speed * Vector3.forward);
        movement.y -= m_Gravity * Time.deltaTime;

        m_characterController.Move(movement * Time.deltaTime);
    }

    private Quaternion CalculateOrientation()
    {
        float rotation = Mathf.Atan2(m_MoveValue.axis.x, m_MoveValue.axis.y);
        rotation *= Mathf.Rad2Deg;

        Vector3 orientationEuler = new Vector3(0, m_Head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);
    }

    private void SnapRotation()
    {
        float snapValue = 0.0f;

        if (m_snapLeftAction.GetStateDown(SteamVR_Input_Sources.Any))
            snapValue = -Mathf.Abs(m_RotateIncrement);
        if (m_snapRightAction.GetStateDown(SteamVR_Input_Sources.Any))
            snapValue = Mathf.Abs(m_RotateIncrement);

        //possible Improv: Add SteamVR Fading
        transform.RotateAround(m_Head.position, Vector3.up, snapValue);
    }
    
}
