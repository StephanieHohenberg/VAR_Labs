using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandPoseController : MonoBehaviour
{
    //Rotationfix should be used to do the raycast from the "circle" of the controller (WMR specific)

    //this script should handle Pose for both hands
    //a raycast will be displayed from the last "used" hand
    //

    public GameObject m_Pointer;
    public SteamVR_Action_Boolean m_TeleportAction;

    private Transform m_Player = null;
    private SteamVR_Behaviour_Pose m_Pose = null;
    private Transform m_rotationFix = null;

    private bool m_HasPosition = false;
    private bool m_IsTeleporting = false;
    private float m_FadeTime = 0.2f;
    
    private LineRenderer m_lineRenderer = null;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_rotationFix = this.gameObject.transform.GetChild(0);
        m_lineRenderer = GetComponent<LineRenderer>();
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Pointer
        m_HasPosition = UpdatePointer();
        m_Pointer.SetActive(m_HasPosition);

        

        if (m_TeleportAction.GetStateUp(m_Pose.inputSource))
            TryTeleport();
    }

    private void TryTeleport()
    {
        if (!m_HasPosition || m_IsTeleporting)
            return;
        Vector3 headPosition = SteamVR_Render.Top().head.position;

        Vector3 groundPosition = new Vector3(headPosition.x, m_Player.position.y, headPosition.z);

        Vector3 translateVector = m_Pointer.transform.position - groundPosition;

        // Move
        StartCoroutine(MovePlayer(translateVector));
    }

    private IEnumerator MovePlayer(Vector3 translation)
    {
        m_IsTeleporting = true;

        SteamVR_Fade.Start(Color.black, m_FadeTime, true);
        //wait until black
        yield return new WaitForSeconds(m_FadeTime);

        m_Player.position += translation;

        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        m_IsTeleporting = false;
    }

    private void SetupPointer()
    {
        //Set Length, Width, Material of the LineRenderer
    }
    private bool UpdatePointer()
    {
        m_lineRenderer.SetPosition(0, m_rotationFix.position);
        Vector3 endPos = m_rotationFix.position + m_rotationFix.forward * 20;
        m_lineRenderer.SetPosition(1, endPos);

        //Raycast from Controller
        Ray ray = new Ray(transform.position, m_rotationFix.forward);
        RaycastHit hit;

        //if its a hit
        if(Physics.Raycast(ray, out hit))
        {
            m_Pointer.transform.position = hit.point;
            m_lineRenderer.SetPosition(1, hit.point);

            string hittag = hit.transform.gameObject.tag;
            //check Tag of the Hit and decide actions

            return true;
        }


        //if not
        return false;
    }

    void HandleGrip()
    {

    }
    
    void DoJediPull()
    {
        //when holding an object through a raycast/laserpointer x
    }
}
