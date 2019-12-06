using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandScript : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    private Interactable m_CurrentInteractable = null;
    Rigidbody targetBody = null;
    private List<Interactable> m_ContactInteractables = new List<Interactable>();

    // Start is called before the first frame update
    void Start()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            Debug.Log(m_Pose.inputSource + "Trigger Down");
            Pickup();
        }

        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            Debug.Log(m_Pose.inputSource + "Trigger Up");
            Drop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;
        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;
        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
    }

    public void Pickup()
    {
        //TODO: If holding, make Physics ignore the Playercharacter
        m_CurrentInteractable = GetNearestInteractable();

        if (!m_CurrentInteractable)
            return;

        //already held, check
        if (m_CurrentInteractable.m_ActiveHand)
            m_CurrentInteractable.m_ActiveHand.Drop();

        //position
        m_CurrentInteractable.transform.position = transform.position;

        //attach
        targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        m_Joint.connectedBody = targetBody;

        //set active hand
        m_CurrentInteractable.m_ActiveHand = this;
    }

    public void Drop()
    {
        if (!m_CurrentInteractable)
            return;

        targetBody.velocity = m_Pose.GetVelocity();
        targetBody.angularVelocity = m_Pose.GetVelocity();

        m_Joint.connectedBody = null;

        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach(Interactable interactable in m_ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }

        return nearest;
    }
}
