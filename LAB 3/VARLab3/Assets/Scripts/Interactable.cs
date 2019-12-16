using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public HandScript m_ActiveHand = null;
    private Rigidbody m_Rigidbody = null;

    public bool isMaster;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ActiveHand != null)
        {
            this.gameObject.layer = 9;
        }
        else
        {
            this.gameObject.layer = 8;
        }
    }

    public void SetKinematic(bool value)
    {
        m_Rigidbody.isKinematic = value;
    }
}
