using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelectorController : MonoBehaviour
{
	public Color selectedColor;
	private Vector3 initPosition;
	string[] paletteTags = {"Cylinder_T", "Cylinder_L", "Cylinder_R", "Cylinder_BL", "Cylinder_BR"};
    Color[] colors = {Color.magenta, Color.yellow, Color.cyan, Color.green, Color.red};
     	

    void Start()
    {
    	selectedColor = Color.white;
    	initPosition = GameObject.FindGameObjectWithTag(paletteTags[0]).transform.position;

		for (int i = 0; i < paletteTags.length; i++)
		{
			initColorOfPalette(paletteTags[i], colors[i]);   
			initColorOfPalette(paletteTags[i], initPosition);   
		}
    }

    void Update()
    { 
    }

    void initColorOfPalette(string tag, Color color) 
    {
    	gameObject = GameObject.FindGameObjectWithTag(tag); 
		renderer = gameObject.GetComponent<Renderer>();
       	GetComponent<Renderer>().material.SetColor("_Color", color);
    }

    void initPositionOfPalette(string tag, Vector3 position) 
    {
    	// TODO
    } 
    


    void onClickOnPalette(GameObject palette) 
    {
    	renderer = palette.GetComponent<Renderer>();
    	selectedColor = GetComponent<Renderer>().getColor("_Color");

    	// hide color selector
    	colorSelector = GameObject.FindGameObjectWithTag("ColorSelector"); 
    	colorSelector.SetActive(false);

    	// add paint can to hand
    	paintCan = GameObject.FindGameObjectWithTag("PaintCan");
    	controllerR = GameObject.FindGameObjectWithTag("ControllerR");
    	paintCan.transform.position = controllerR.transform.position;
    	// TODO animation
    }

    void processInput() 
    {
    	// TODO call onClickOnPalette
    }


    // TODO after click on Object 
    // Color Object in Color and reset selectedColor to null

}
