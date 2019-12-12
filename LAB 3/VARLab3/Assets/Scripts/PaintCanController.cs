using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCanController : MonoBehaviour
{
	public GameObject pointer;
    public GameObject spray;
    public GameObject colorSelector;
    private Color color = Color.red;

    private LineRenderer lineRenderer = null;
    private Transform rotationFix = null;


    void Awake()
    {
        rotationFix = this.gameObject.transform.GetChild(0);
        lineRenderer = GetComponent<LineRenderer>();
    
        changeColorOfGameObject(spray, color);
        changeColorOfGameObject(pointer, color);

        pointer.SetActive(false);
        spray.SetActive(false);
        colorSelector.SetActive(false);
    }


    void Start()
    {
    }

    void Update() 
    {
    	if( pointer.activeSelf )  
    	{
    		var hitGameObject = updatePositionToHandAndCheckForHitGameobject(pointer);
    		if(hitGameObject != null) 
    		{
    			onPointerHit(hitGameObject);
    		}

    	}

    	if( spray.activeSelf )  
    	{
    		var hitGameObject = updatePositionToHandAndCheckForHitGameobject(spray);
    		if(hitGameObject != null) 
    		{
    			onSrayHit(hitGameObject);
    		}

    	}
	}


    public void onAttach() 
    {
        pointer.SetActive(true);   
    }
  

   	public void onDeattach() 
    {
        pointer.SetActive(false);   
    }

    public void onSpray() 
    {
        onCloseColorMenu();
    	onDeattach();
        spray.SetActive(true);
    }

	public void onSprayStop() 
    {
    	spray.SetActive(false);
    	onAttach();
    }

    public void onOpenColorMenu() 
    {
        updatePositionOfColorMenu();
        colorSelector.SetActive(true);
    }

    public void onCloseColorMenu() 
    {
        colorSelector.SetActive(false);
    }

    private GameObject updatePositionToHandAndCheckForHitGameobject(GameObject sprayOrPointer)
    {
        lineRenderer.SetPosition(0, rotationFix.position);
        Vector3 endPos = rotationFix.position + rotationFix.forward * 20;
        lineRenderer.SetPosition(1, endPos);

        //Raycast from Controller
        Ray ray = new Ray(transform.position, rotationFix.forward);
        RaycastHit hit;

        //if its a hit
        if(Physics.Raycast(ray, out hit))
        {
            sprayOrPointer.transform.position = hit.point;
            lineRenderer.SetPosition(1, hit.point);

            return hit.transform.gameObject; 
        }

        return null;
    }

    private void updatePositionOfColorMenu()
    {
        colorSelector.transform.position = pointer.transform.position; 
        // TODO
    }

    private void onPointerHit(GameObject gameObject) 
    {
    	if(gameObject.transform.parent != null && gameObject.transform.parent.tag == "ColorSelector")
    	{
	    	var renderer = gameObject.GetComponent<Renderer>();
    		color = renderer.material.GetColor("_Color");

    		changeColorOfGameObject(spray, color);
    		changeColorOfGameObject(pointer, color);
    	}
    }

    private void onSrayHit(GameObject gameObject) 
    {
    	if(gameObject.tag == "Interactable") {
    		changeColorOfGameObject(gameObject, color);
    	} 
    }

    
    private void changeColorOfGameObject(GameObject gameObject, Color color) 
    {
    	var renderer = gameObject.GetComponent<Renderer>();
    	renderer.material.SetColor("_Color", color);
    }


}
