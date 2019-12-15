using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandPoseController : MonoBehaviour
{
    //this script should handle Pose for both hands
    //a raycast will be displayed from the last "used" hand
    
        //General 

    private Transform m_Player = null;
    private SteamVR_Behaviour_Pose m_Pose = null;

    //Rotationfix should be used to do the raycast from the "circle" of the controller (WMR specific)
    private Transform m_rotationFix = null;
    private LineRenderer m_lineRenderer = null;
    private bool m_HasPosition = false;
    public GameObject m_Pointer;

    //Teleporting
    public SteamVR_Action_Boolean m_TeleportAction;
    private bool m_IsTeleporting = false;
    private float m_FadeTime = 0.2f;

    //Grabbing & Interaction
    //keep the Scripts (at least a bit) semantically separated
    // forward Global Variables like IsTeleporting or Rotationfix
    public HandScript m_HandScript = null;
    


    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_rotationFix = this.gameObject.transform.GetChild(0);
        m_lineRenderer = GetComponent<LineRenderer>();
        m_HandScript = GetComponent<HandScript>();
        m_HandScript.m_rotationFix = this.m_rotationFix;
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
        if (!m_HasPosition || m_IsTeleporting || m_HandScript.m_HasInteractable)
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
            if (hit.transform.CompareTag("Interactable"))
            {

            }

            return true;
        }
        //if not
        return false;
    }
    
}
