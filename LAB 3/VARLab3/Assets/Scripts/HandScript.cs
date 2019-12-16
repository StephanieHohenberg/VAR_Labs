using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandScript : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Action_Boolean m_SecondaryAction = null;
    public SteamVR_Action_Single m_PushAction = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    //Interactables
    private Interactable m_CurrentInteractable = null;
    public Rigidbody m_PointingAt = null;
    public bool m_HasInteractable = false;
    private PaintCanController m_Cancontroller;

    Rigidbody targetBody = null;
    private List<Interactable> m_ContactInteractables = new List<Interactable>();

    //From HandPoseController
    public Transform m_rotationFix = null;
    public GameObject gallery = null;
    public Vector3 galleryReset = new Vector3(0, -2, 0);

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
            if (m_CurrentInteractable == null)
                Pickup(); //BUG: Can take the last object out of gallery multiple times
            else
                Drop();
        }

        if (m_SecondaryAction.GetStateUp(m_Pose.inputSource))
            DoSecondary();

        if (m_PushAction.GetAxis(m_Pose.inputSource) != 0f)
        {
            DoPrimary();
        }
        else if (m_Cancontroller != null) {
            m_Cancontroller.onSprayStop();
        }
    }
    private void DoSecondary()
    {

        if (!m_HasInteractable)
        {
            PlaceGallery();
        }
        else if (m_Cancontroller != null)
        {
            m_Cancontroller.onToggleColorMenu(); 
        }
        else
        {
            //Drop it fixed in space
            Drop(freeze: true);
        }

    }

    private void DoPrimary()
    {
        if (!m_HasInteractable && m_PointingAt != null)
        {
            Debug.Log("TryPushSomething:"+m_PointingAt.position);
            TryPullSomething(m_PointingAt);
        }
        else if (m_Cancontroller != null)
        {
            m_Cancontroller.onSpray();
        }
        else if (m_HasInteractable)
        {
            float pushForce = m_PushAction.GetAxis(m_Pose.inputSource);
            UseInteractable(pushForce);
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

    public void Pickup(Interactable forced = null)
    {
        if (forced == null)
        {
            m_CurrentInteractable = GetNearestInteractable();
        } else
        {
            m_CurrentInteractable = forced;
        }

        if (!m_CurrentInteractable)
            return;

        if (m_CurrentInteractable.isMaster)
        {
            TakeFromGallery();
        }

        //already held, check
        if (m_CurrentInteractable.m_ActiveHand)
            m_CurrentInteractable.m_ActiveHand.Drop();

        //position
        m_CurrentInteractable.transform.position = transform.position;
        m_CurrentInteractable.transform.LookAt(transform.position+ m_rotationFix.forward);

        //attach
        targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.isKinematic = false;
        m_Joint.connectedBody = targetBody;

        //set active hand
        m_CurrentInteractable.m_ActiveHand = this;
        m_HasInteractable = true;

        //get paintCanController if Possible
        m_CurrentInteractable.TryGetComponent<PaintCanController>(out m_Cancontroller);
        if (m_Cancontroller != null) {
            m_Cancontroller.onAttach();
        }
    }

    public void Drop(bool freeze=false)
    {
        if (!m_CurrentInteractable)
            return;

        targetBody.velocity = m_Pose.GetVelocity();
        targetBody.angularVelocity = m_Pose.GetVelocity();
        targetBody.isKinematic = freeze;
        m_Joint.connectedBody = null;

        if (m_Cancontroller != null)
        {
            m_Cancontroller.onDeattach();
        }

        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;
        m_Cancontroller = null;
        m_HasInteractable = false;
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

    private void UseInteractable(float pushForce)
    {
        if (pushForce > 0.3f)
        {
            RehingeJoint();
        }


        //targetBody.transform.LookAt(m_rotationFix.forward);
        //m_Joint.connectedBody = m_CurrentInteractable.GetComponent<Rigidbody>();
    }

    private void RehingeJoint()
    {
        // Save end of hinge, then unscrew it:
        Rigidbody RB = m_Joint.connectedBody;
        m_Joint.connectedBody = null;
        // Can now move unconnected hinge:
        RB.transform.LookAt(m_rotationFix.position + m_rotationFix.forward*20);
        RB.transform.position += m_rotationFix.forward * 0.01f;

        m_Joint.connectedBody = RB; // screw it back in
    }

    private void PlaceGallery()
    {
        gallery.transform.position = m_rotationFix.position +  m_rotationFix.forward* 0.3f;
        gallery.transform.LookAt(m_rotationFix.position + m_rotationFix.forward);
        gallery.SetActive(true);
    }

    private void TakeFromGallery()
    {
        GameObject copyGO = Instantiate(m_CurrentInteractable.gameObject);
        m_ContactInteractables.Remove(m_CurrentInteractable);
        copyGO.transform.position = m_CurrentInteractable.transform.position;
        m_CurrentInteractable = copyGO.GetComponent<Interactable>();
        m_CurrentInteractable.isMaster = false;
        gallery.transform.position = galleryReset;
        gallery.SetActive(false);
    }

    private void TryPullSomething(Rigidbody obj)
    {
        Debug.Log("Inside method");
        Vector3 Vec = (obj.transform.position - transform.position);
        if (Vec.magnitude <= 0.2)
        {
            Debug.Log(Vec.magnitude);
            Pickup(obj.gameObject.GetComponent<Interactable>());
        }
        else
        {
            Debug.Log("ChangePosition");
            obj.transform.LookAt(m_rotationFix.position - m_rotationFix.forward);
            //obj.AddForce(-obj.transform.position + transform.position);
            obj.transform.position -= m_rotationFix.forward * Vec.magnitude/10;
            obj.velocity = (m_rotationFix.position - obj.transform.position);
        }
        // It has a Magnitude Depending on the Distance, otherwise use normal * factor
    }
}
