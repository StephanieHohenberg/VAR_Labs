using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintCanController : MonoBehaviour
{
    public GameObject spray;
    public GameObject colorSelector;
    private Color color = Color.red;

    private LineRenderer lineRenderer = null;
    private bool isCoroutineExecuting = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        ParticleSystem.MainModule settings = spray.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient( color );

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
            if ( spray.activeSelf && isCoroutineExecuting == false)  
    	    {
                StartCoroutine(onSprayHit(hitGameObject));
    			
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
        colorSelector.SetActive(false);
        onDeattach();
        spray.SetActive(true);
    }

	public void onSprayStop() 
    {
    	spray.SetActive(false);
    	onAttach();
    }

    public void onToggleColorMenu() 
    {
        if(colorSelector.activeSelf) 
        {
            colorSelector.SetActive(false);
        } 
        else 
        {
            updatePositionOfColorMenu();
            colorSelector.SetActive(true);
        }
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
        colorSelector.transform.LookAt(transform.position + 10 * transform.forward);
    }

    private void onPointerHit(GameObject gameObject) 
    {
    	if(gameObject.transform.parent != null && gameObject.transform.parent.tag == "ColorSelector")
    	{
	    	var renderer = gameObject.GetComponent<Renderer>();
    		color = renderer.material.GetColor("_Color");

            ParticleSystem.MainModule settings = spray.GetComponent<ParticleSystem>().main;
            settings.startColor = new ParticleSystem.MinMaxGradient( color );

            changeColorOfGameObject(spray, color);

            lineRenderer.material = renderer.material;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

    	}
    }

    private IEnumerator onSprayHit(GameObject gameObject) 
    {
 
        isCoroutineExecuting = true;
        yield return new WaitForSeconds(5.0f);

    	if(gameObject.tag == "Interactable") {
    		changeColorOfGameObject(gameObject, color);
    	} 

        isCoroutineExecuting = false;
    }

    
    private void changeColorOfGameObject(GameObject gameObject, Color color) 
    {
        var renderer = gameObject.GetComponent<Renderer>();
    	renderer.material.SetColor("_Color", color);
    }


}
