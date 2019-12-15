using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCanController : MonoBehaviour
{
    public GameObject spray;
    public GameObject colorSelector;
    private Color color = Color.red;

    private LineRenderer lineRenderer = null;


    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    
        changeColorOfGameObject(spray, color);

        lineRenderer.enabled = false;
        spray.SetActive(false);
        colorSelector.SetActive(false);
    }


    void Start()
    {
    }

    void Update() 
    {
        var hitGameObject = updateRaycastAndCheckForHitGameobject();
        if (hitGameObject != null)
        {
            if ( spray.activeSelf )  
    	    {
    		
    			onSrayHit(hitGameObject);
    		} else
            {
                onPointerHit(hitGameObject);
            }

    	}
	}


    public void onAttach() 
    {
        lineRenderer.enabled = true;
    }
  

   	public void onDeattach() 
    {
        lineRenderer.enabled = false;
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

    private GameObject updateRaycastAndCheckForHitGameobject()
    {
        Vector3 CanOpening = transform.position + transform.up * 0.09f;
        lineRenderer.SetPosition(0, CanOpening);
        Vector3 endPos = CanOpening + transform.forward * 5;
        lineRenderer.SetPosition(1, endPos);

        //Raycast from Controller
        Ray ray = new Ray(CanOpening, transform.forward);
        RaycastHit hit;

        //if its a hit
        if(Physics.Raycast(ray, out hit))
        {
            lineRenderer.SetPosition(1, hit.point);
            return hit.transform.gameObject; 
        }

        return null;
    }

    private void updatePositionOfColorMenu()
    {
        colorSelector.transform.position = transform.position+transform.forward;
        colorSelector.transform.LookAt(transform.position + 2 * transform.forward);
    }

    private void onPointerHit(GameObject gameObject) 
    {
    	if(gameObject.transform.parent != null && gameObject.transform.parent.tag == "ColorSelector")
    	{
	    	var renderer = gameObject.GetComponent<Renderer>();
    		color = renderer.material.GetColor("_Color");

    		changeColorOfGameObject(spray, color);
            lineRenderer.startColor=color;
            lineRenderer.endColor = color;
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
