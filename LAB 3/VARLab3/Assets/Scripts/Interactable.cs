using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public HandScript m_ActiveHand = null;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
