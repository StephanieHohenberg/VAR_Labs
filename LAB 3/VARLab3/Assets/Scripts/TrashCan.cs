using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    // Start is called before the first frame update
    private Interactable currentNear = null;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;
        other.gameObject.TryGetComponent<Interactable>(out currentNear);
        if (currentNear.m_ActiveHand != null)
            return;
        Destroy(other.gameObject);
    }
    
}
